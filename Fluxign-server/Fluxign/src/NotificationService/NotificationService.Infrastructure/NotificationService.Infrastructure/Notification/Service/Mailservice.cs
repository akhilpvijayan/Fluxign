using Microsoft.AspNetCore.Components.Routing;
using NotificationService.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using UserService.Infrastructure.Notification.Util;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NotificationService.Infrastructure.Notification.Service
{
    public class Mailservice : IMailService
    {
        public async Task SendOtp(string otpCode, string otpPurpose, string email, string username)
        {
            try
            {
                string subject = $"🔐 Your verification code for {otpPurpose}";

                string msg = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>OTP Verification</title>
        </head>
        <body style='margin: 0; padding: 0; background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
                <!-- Header -->
                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                    <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600; letter-spacing: -0.5px;'>Done.ae</h1>
                    <p style='color: #e2e8f0; margin: 8px 0 0 0; font-size: 16px;'>Verification Required</p>
                </div>
                
                <!-- Main Content -->
                <div style='padding: 50px 30px 40px 30px;'>
                    <div style='text-align: center; margin-bottom: 40px;'>
                        <div style='width: 80px; height: 80px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; margin: 0 auto 20px auto; display: flex; align-items: center; justify-content: center;'>
                            <span style='color: #ffffff; font-size: 36px;'>🔐</span>
                        </div>
                        <h2 style='color: #1e293b; margin: 0 0 12px 0; font-size: 24px; font-weight: 600;'>Verify Your Identity</h2>
                        <p style='color: #64748b; margin: 0; font-size: 16px; line-height: 1.5;'>Hi <strong style='color: #334155;'>{username}</strong>, use the code below to complete your <strong style='color: #334155;'>{otpPurpose}</strong></p>
                    </div>
                    
                    <!-- OTP Code Box -->
                    <div style='background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%); border: 2px dashed #cbd5e1; border-radius: 16px; padding: 30px; text-align: center; margin: 30px 0; position: relative; overflow: hidden;'>
                        <div style='position: absolute; top: -50%; left: -50%; width: 200%; height: 200%; background: radial-gradient(circle, rgba(102, 126, 234, 0.05) 0%, transparent 70%); pointer-events: none;'></div>
                        <p style='color: #64748b; margin: 0 0 12px 0; font-size: 14px; font-weight: 500; text-transform: uppercase; letter-spacing: 1px;'>Your Verification Code</p>
                        <div style='font-size: 36px; font-weight: 700; color: #667eea; letter-spacing: 8px; margin: 8px 0; font-family: ""SF Mono"", Monaco, ""Cascadia Code"", ""Roboto Mono"", Consolas, ""Courier New"", monospace; position: relative;'>{otpCode}</div>
                        <p style='color: #94a3b8; margin: 12px 0 0 0; font-size: 12px;'>Valid for 5 minutes</p>
                    </div>
                    
                    <!-- Security Notice -->
                    <div style='background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 20px; border-radius: 0 8px 8px 0; margin: 30px 0;'>
                        <div style='display: flex; align-items: flex-start;'>
                            <span style='color: #f59e0b; font-size: 20px; margin-right: 12px; flex-shrink: 0;'>⚠️</span>
                            <div>
                                <p style='color: #92400e; margin: 0 0 8px 0; font-weight: 600; font-size: 14px;'>Security Notice</p>
                                <p style='color: #a16207; margin: 0; font-size: 13px; line-height: 1.4;'>Never share this code with anyone. Done.ae will never ask for your verification code via phone or email.</p>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Help Text -->
                    <div style='text-align: center; margin-top: 40px;'>
                        <p style='color: #94a3b8; font-size: 14px; line-height: 1.6; margin: 0;'>
                            Didn't request this code? You can safely ignore this email.<br>
                            Need help? <a href='mailto:support@done.ae' style='color: #667eea; text-decoration: none; font-weight: 500;'>Contact Support</a>
                        </p>
                    </div>
                </div>
                
                <!-- Footer -->
                <div style='background-color: #f8fafc; padding: 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                    <p style='color: #64748b; font-size: 13px; margin: 0 0 12px 0; line-height: 1.5;'>
                        This email was sent to <strong style='color: #475569;'>{email}</strong><br>
                        © 2025 Done.ae. All rights reserved.
                    </p>
                    <div style='margin-top: 20px;'>
                        <a href='#' style='color: #94a3b8; text-decoration: none; font-size: 12px; margin: 0 12px;'>Privacy Policy</a>
                        <span style='color: #cbd5e1;'>•</span>
                        <a href='#' style='color: #94a3b8; text-decoration: none; font-size: 12px; margin: 0 12px;'>Terms of Service</a>
                        <span style='color: #cbd5e1;'>•</span>
                        <a href='#' style='color: #94a3b8; text-decoration: none; font-size: 12px; margin: 0 12px;'>Unsubscribe</a>
                    </div>
                </div>
            </div>
        </body>
        </html>";

                if (!string.IsNullOrWhiteSpace(email))
                {
                    NotificationUtil.SendMail(email, msg, subject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendPasswordReset(string resetLink, string email)
        {
            try
            {
                string subject = "🔐 Reset Your Password";

                string msg = $@"
<div style=""max-width: 600px; margin: auto; background-color: #fff; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen,
Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 6px rgba(0,0,0,0.1);"">
  <div style=""padding: 30px 40px; background-color: #f5f7fa; border-bottom: 1px solid #e1e4e8;"">
    <h2 style=""margin: 0; color: #222; font-weight: 700;"">Reset Your Password</h2>
    <p style=""margin-top: 10px; color: #555; font-size: 16px;"">We received a request to reset your password. Click the button below to continue.</p>
  </div>

  <div style=""padding: 30px 40px;"">
    <p style=""color: #333; font-size: 15px; margin-bottom: 20px;"">To reset your password, click this button:</p>

    <div style=""text-align: center; margin: 30px 0;"">
      <a href=""{resetLink}"" 
         style=""display: inline-block; background-color: #6bc91f; color: #fff; padding: 14px 28px; font-size: 16px; font-weight: 600; border-radius: 6px; text-decoration: none; box-shadow: 0 3px 8px rgba(107, 201, 31, 0.4); transition: background-color 0.3s ease;"">
        Reset Password
      </a>
    </div>

    <p style=""color: #777; font-size: 14px; margin-bottom: 10px;"">If the button doesn’t work, copy and paste the following link into your browser:</p>
    <p style=""word-break: break-word; font-size: 14px; color: #1a73e8;"">
      <a href=""{resetLink}"" style=""color: #1a73e8; text-decoration: none;"">{resetLink}</a>
    </p>

    <p style=""margin-top: 30px; color: #999; font-size: 13px;"">This link will expire in 30 minutes for your security.</p>

    <p style=""margin-top: 20px; color: #444; font-size: 14px;"">If you didn’t request a password reset, you can safely ignore this email.</p>

  </div>

  <div style=""padding: 20px 40px; background-color: #f5f7fa; text-align: center; font-size: 12px; color: #999;"">
    <p style=""margin: 0;"">© {DateTime.UtcNow.Year} Done.ae. All rights reserved.</p>
  </div>
</div>";

                if (!string.IsNullOrWhiteSpace(email))
                {
                    NotificationUtil.SendMail(email, msg, subject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendSigningLink(string requestTitle, string email, string firstName, string signingUrl)
        {
            try
            {

                string subject = $"📝 Action Required: {requestTitle}";

                string msg = $@"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8'>
                                  <title>Sign Document</title>
                                </head>
                                <body style='margin:0; padding:0; background-color:#f8fafc; font-family:Arial,sans-serif;'>
                                  <div style='max-width:600px; margin:0 auto; background-color:#ffffff;'>

                                    <!-- Header -->
                                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding:40px 30px; text-align:center;'>
                                      <h1 style='color:#ffffff; margin:0; font-size:28px; font-weight:600;'>Done.ae</h1>
                                      <p style='color:#e2e8f0; font-size:16px; margin:8px 0 0 0;'>Document Signature Request</p>
                                    </div>

                                    <!-- Main Content -->
                                    <div style='padding: 40px 30px;'>
                                      <p style='font-size:18px; color:#1e293b; margin-bottom:16px;'>Hi <strong>{firstName}</strong>,</p>
                                      <p style='font-size:16px; color:#334155; line-height:1.6;'>
                                        You are requested to sign a document for <strong>{requestTitle}</strong>. Please choose an action below:
                                      </p>

                                      <!-- Buttons -->
                                      <div style='text-align:center; margin:40px 0;'>
                                        <a href='{signingUrl}&status=accept'
                                           style='display:inline-block; margin:10px; padding:14px 28px; background-color:#6bc91f; color:#ffffff; font-size:16px; font-weight:600; text-decoration:none; border-radius:8px; transition:all 0.3s ease;'>
                                          ✓ Sign Document
                                        </a>
                                        <a href='{signingUrl}&status=reject'
                                           style='display:inline-block; margin:10px; padding:14px 28px; background-color:#ef4444; color:#ffffff; font-size:16px; font-weight:600; text-decoration:none; border-radius:8px; transition:all 0.3s ease;'>
                                          ✗ Reject Request
                                        </a>
                                      </div>

                                      <!-- Help Text -->
                                      <div style='text-align:center; margin-top:30px;'>
                                        <p style='font-size:14px; color:#64748b;'>Having trouble with the buttons?</p>
                                        <p style='font-size:14px;'>
                                          <a href='{signingUrl}&status=accept' style='color:#6bc91f; font-weight:600; text-decoration:none;'>Click here to Sign</a>
                                           or
                                          <a href='{signingUrl}&status=reject' style='color:#ef4444; font-weight:600; text-decoration:none;'>Click here to Reject</a>
                                        </p>
                                      </div>

                                      <!-- Direct Links with Copy Icon -->
                                      <div style='margin-top:30px; padding:20px; background-color:#f1f5f9; border-radius:8px;'>
                                        <p style='font-size:14px; color:#1e293b; font-weight:bold;'>🔗 Direct Links</p>
                                        <p style='font-size:13px; color:#475569;'>Copy and paste these URLs if buttons don’t work:</p>
                                        <p style='font-size:13px; color:#059669; background:#f0fdf4; padding:10px; border-radius:6px; font-family:monospace; word-break:break-all;'>
                                          📝 To Sign: {signingUrl}&status=accept
                                        </p>
                                        <p style='font-size:13px; color:#dc2626; background:#fef2f2; padding:10px; border-radius:6px; font-family:monospace; word-break:break-all;'>
                                          📝 To Reject: {signingUrl}&status=reject
                                        </p>
                                      </div>

                                      <!-- Security Notice -->
                                      <div style='margin-top:30px; padding:20px; background-color:#fef3c7; border-left:4px solid #f59e0b; border-radius:6px;'>
                                        <p style='margin:0; font-size:14px; color:#92400e; font-weight:600;'>🔒 Security Notice</p>
                                        <p style='margin:8px 0 0 0; font-size:13px; color:#a16207;'>
                                          Only click these links if you initiated the request. Done.ae will never ask for sensitive information via email.
                                        </p>
                                      </div>
                                    </div>

                                    <!-- Footer -->
                                    <div style='text-align:center; padding:30px; background-color:#f8fafc; border-top:1px solid #e2e8f0;'>
                                      <p style='font-size:13px; color:#64748b; margin:0 0 12px 0;'>This email was sent to <strong>{email}</strong></p>
                                      <p style='font-size:12px; color:#94a3b8; margin:0 0 12px 0;'>© 2025 Done.ae. All rights reserved.</p>
                                      <p style='font-size:12px; color:#94a3b8; margin:0;'>
                                        <a href='#' style='color:#94a3b8; text-decoration:none; margin:0 8px;'>Privacy Policy</a> •
                                        <a href='#' style='color:#94a3b8; text-decoration:none; margin:0 8px;'>Terms of Service</a> •
                                        <a href='#' style='color:#94a3b8; text-decoration:none; margin:0 8px;'>Unsubscribe</a>
                                      </p>
                                    </div>
                                  </div>
                                </body>
                                </html>";
                if (!string.IsNullOrWhiteSpace(email))
                {
                    NotificationUtil.SendMail(email, msg, subject);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
