using RequestService.Application.DTOs;
using RequestService.Application.Interfaces.Repositories;
using RequestService.Application.Interfaces.Services;
using RequestService.Domain.Entities;
using RequestService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RequestService.Application.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;
using NotificationService.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace RequestService.Application.Services
{
    public class SigningRequestService : ISigningRequestService
    {
        private readonly ISigningRequestsRepository _signingRequestsRepository;
        private readonly IDocumentServiceClient _documentServiceClient;
        private readonly ISigningRecepientsRepository _signingRecepientsRepository;
        private readonly ISignaturePositionsRepository _signaturePositionsRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly INotificationServiceClient _notificationServiceClient;
        private readonly string _frontendUrl;
        public SigningRequestService(ISigningRequestsRepository signingRequestsRepository, IDocumentServiceClient documentServiceClient, 
            ISigningRecepientsRepository signingRecepientsRepository, ISignaturePositionsRepository signaturePositionsRepository, 
            IUserServiceClient userServiceClient, INotificationServiceClient notificationServiceClient, IConfiguration configuration)
        {
            _signingRequestsRepository = signingRequestsRepository;
            _documentServiceClient = documentServiceClient;
            _signingRecepientsRepository = signingRecepientsRepository;
            _signaturePositionsRepository = signaturePositionsRepository;
            _userServiceClient = userServiceClient;
            _notificationServiceClient = notificationServiceClient;
            _frontendUrl = configuration["Frontend:BaseUrl"];
        }

        public async Task<ServiceResult<List<RequestDashboardDto>>> GetDashboardByUserIdAsync(Guid userId, DashboardQueryParameterDto pagingparams)
        {
            try
            {
                var requests = await _signingRequestsRepository.GetDashboardByUserIdAsync(userId, pagingparams);

                foreach (var request in requests)
                {
                    if(request.Recipients != null)
                    {
                        foreach (var recipient in request?.Recipients)
                        {
                            var user = await _userServiceClient.GetUserByEmailAsync(recipient.Email);
                            if (user != null && !string.IsNullOrWhiteSpace(user.AvatarImage))
                            {
                                recipient.AvatarUrl = user.AvatarImage;
                            }
                            else
                            {
                                recipient.AvatarUrl = GenerateInitialsAvatarSvg(recipient.Name);
                            }
                        }
                    }
                }
                return ServiceResult<List<RequestDashboardDto>>.Success(requests, "Requests fetched successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<RequestDashboardDto>>.Failure("Requests feching failed.");
            }
        }

        public async Task<ServiceResult<List<SigningRequest>>> GetAllRequestsByUserIdAsync(Guid userId)
        {
            try
            {
                var requests = await _signingRequestsRepository.GetAllRequestsByUserIdAsync(userId);
                return ServiceResult<List<SigningRequest>>.Success(requests, "Requests fetched successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<SigningRequest>>.Failure("Requests feching failed.");
            }

        }

        public async Task<ServiceResult<SigningRecipient>> GetRecipientByToken(string token)
        {
            try
            {
                var signer = await _signingRecepientsRepository.GetRecipientByTokenAsync(token);
                return ServiceResult<SigningRecipient>.Success(signer, "Signer details fetched successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<SigningRecipient>.Failure("Signer details feching failed.");
            }
        }

        public async Task<ServiceResult<SigningRequestDto>> GetRequestByIdAsync(Guid userId, Guid requestId)
        {
            try
            {
                var request = await _signingRequestsRepository.GetRequestByIdAsync(userId, requestId);
                var recepients = await _signingRecepientsRepository.GetRecepientsById(requestId);
                var document = await _documentServiceClient.GetDocumentByRequestId(requestId);

                SigningRequestDto signingRequest = new SigningRequestDto()
                {
                    RequestId = request.Id,
                    Title = request.Title,
                    FileName = document?.OriginalFileName,
                    PdfBase64 = document?.OriginalFile
                };

                foreach (var item in recepients)
                {
                    Signer signer = new Signer()
                    {
                        Email = item.RecipientEmail,
                        Mobile = item.RecipientPhone,
                        Name = item.RecipientName,
                        Rank = item.SigningOrder,
                    };

                    var signaturePosition = await _signaturePositionsRepository.GetPositionsByRecepientId(item.Id, item.RequestId);

                    Position position = new Position()
                    {
                        X = (double)signaturePosition.XPosition,
                        Y = (double)signaturePosition.YPosition,
                        Page = signaturePosition.PageNumber
                    };

                    signer.Position = position;
                    if(signingRequest.Signers == null)
                    {
                        signingRequest.Signers = new List<Signer>();
                    }
                    signingRequest.Signers.Add(signer);
                }

                return ServiceResult<SigningRequestDto>.Success(signingRequest, "Request fetching success.");
            }
            catch (Exception ex)
            {
                return ServiceResult<SigningRequestDto>.Failure("Request fetching failed.");
            }
        }

        public async Task<ServiceResult<Guid>> CreateSigningRequestAsync(SigningRequestDto request)
        {
            try
            {
                if (!Enum.IsDefined(typeof(SigningStatus), request.Status))
                {
                    throw new ArgumentException($"Invalid status value: {request.Status}");
                }

                var statusEnum = (SigningStatus)request.Status;
                var data = new SigningRequest
                {
                    RequesterId = request.UserId,
                    Title = request.Title,
                    Status = statusEnum.ToString(),
                    IsSequentialSigning = true,
                    TotalRecipients = request.Signers.Count,
                    CompletedSigners = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = request.UserId
                };

                await _signingRequestsRepository.AddAsync(data);

                foreach(var item in request.Signers)
                {
                    await UpdateSignerAndPositions(request, data, item);
                }

                request.RequestId = data.Id;
                var docSaved = await _documentServiceClient.CreateDocumentAsync(request);

                if (!docSaved)
                {
                    await _signingRequestsRepository.DeleteAsync(data.Id);
                    return ServiceResult<Guid>.Failure("Request adding failed.");
                }

                //await _hubContext.Clients.Group(requestId.ToString())
                //                                .SendAsync("StatusUpdated", new
                //                                {
                //                                RequestId = requestId,
                //                                NewStatus = newStatus,
                //                                UpdatedAt = DateTime.UtcNow
                //                                });

                if(data.Status == SigningStatus.Pending.ToString())
                {
                    NotificationRequestDto notificationRequest = new NotificationRequestDto()
                    {
                        Email = request?.Signers[0].Email,
                        Name = request?.Signers[0].Name,
                        Title = request.Title,
                        RequestType = (int)NotificationTypeEnum.SigningRequest,
                        RedirectUrl = _frontendUrl + "sign?token=" + _signingRecepientsRepository.GetRecepientsById((Guid)request.RequestId).Result?.Where(x=>x.SigningOrder == 1).FirstOrDefault().SigningToken,
                    };
                    var isEmailSend = await _notificationServiceClient.CreateNotification(notificationRequest);
                    if (!isEmailSend)
                    {
                        return ServiceResult<Guid>.Failure("Email sending failed.");
                    }
                }

                return ServiceResult<Guid>.Success(data.Id, data.Status == SigningStatus.Pending.ToString() ? "Request added successfully." : "Draft saved successfully.");

            }
            catch (Exception ex) 
            {
                return ServiceResult<Guid>.Failure("Request adding failed.");
            }
        }

        public async Task<ServiceResult<Guid>> UpdateSigningRequestAsync(SigningRequestDto request)
        {
            try
            {
                if (!Enum.IsDefined(typeof(SigningStatus), request.Status))
                {
                    throw new ArgumentException($"Invalid status value: {request.Status}");
                }

                var statusEnum = (SigningStatus)request.Status;

                var data = await _signingRequestsRepository.GetRequestByIdAsync(request.UserId, (Guid)request.RequestId);

                data.Title = request.Title;
                data.Status = statusEnum.ToString();
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UserId;

                await _signingRequestsRepository.UpdateAsync(data);

                await _signaturePositionsRepository.DeleteByRequestIdAsync((Guid)request.RequestId);
                await _signingRecepientsRepository.DeleteByRequestIdAsync((Guid)request.RequestId);

                foreach (var item in request.Signers)
                {
                    await UpdateSignerAndPositions(request, data, item);
                }

                return ServiceResult<Guid>.Success(data.Id, data.Status == SigningStatus.Pending.ToString() ? "Request added successfully." : "Draft saved successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Failure("Request adding failed.");
            }
        }

        private string GenerateSigningToken()
        {
            var guid = Guid.NewGuid();
            var bytes = guid.ToByteArray();
            return Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }

        private string GenerateInitialsAvatarSvg(string fullName)
        {
            var initials = string.Join("", fullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w[0])
                .Take(2)).ToUpper();

            var random = new Random();
            string bgColor = $"#{random.Next(0x1000000):X6}";

            var svg = $@"
                        <svg xmlns='http://www.w3.org/2000/svg' width='100' height='100'>
                            <rect width='100' height='100' fill='{bgColor}' />
                            <text x='50%' y='50%' dy='.35em' text-anchor='middle' fill='white' font-size='40' font-family='Arial'>
                                {initials}
                            </text>
                        </svg>";

            var bytes = Encoding.UTF8.GetBytes(svg);
            return $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
        }

        private async Task UpdateSignerAndPositions(SigningRequestDto request, SigningRequest data, Signer item)
        {
            try
            {
                var recepients = new SigningRecipient()
                {
                    RequestId = data.Id,
                    RecipientEmail = item.Email,
                    RecipientName = item.Name,
                    RecipientPhone = item.Mobile,
                    SigningOrder = (int)item.Rank,
                    SigningToken = GenerateSigningToken(),
                    Status = SigningRecipientStatus.Pending.ToString(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };

                await _signingRecepientsRepository.AddAsync(recepients);

                var signaturePosition = new SignaturePosition()
                {
                    RequestId = data.Id,
                    RecipientId = recepients.Id,
                    XPosition = (decimal)item.Position.X,
                    YPosition = (decimal)item.Position.Y,
                    PageNumber = item.Position.Page,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = request.UserId
                };

                await _signaturePositionsRepository.AddAsync(signaturePosition);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
