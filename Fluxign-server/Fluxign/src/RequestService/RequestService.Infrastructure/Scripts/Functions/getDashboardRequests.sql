-- ===========================================================================================
-- FUNCTION     : get_user_dashboard
-- DESCRIPTION  : 
--     Retrieves a paginated list of signing requests created by the specified user.
--     Each signing request includes detailed recipient information along with 
--     their signature positions. Optionally filters results by request status and/or 
--     name of the signing request.
--
-- PARAMETERS   :
--     user_id       UUID        - The ID of the user whose dashboard data is being retrieved.
--     status_filter TEXT        - (Optional) Filters signing requests by status 
--                                 (e.g., 'Pending', 'Completed'). Defaults to NULL (no filter).
--     name_filter   TEXT        - (Optional) Filters signing requests by name 
--                                 (case-insensitive, partial match). Defaults to NULL (no filter).
--     page_number   INT         - (Optional) The page number for pagination. Defaults to 1.
--     page_size     INT         - (Optional) The number of records per page. Defaults to 10.
--
-- RETURNS      : JSONB
--     A JSON array of signing requests including recipient and signature position details,
--     filtered and paginated based on input parameters.
--
-- CREATED BY   : Akhil Padackal Vijayan
-- CREATED ON   : 2025-09-11
-- ===========================================================================================

CREATE OR REPLACE FUNCTION get_user_dashboard(
    user_id UUID,
    status_filter TEXT DEFAULT NULL,
    name_filter TEXT DEFAULT NULL,
    page_number INT DEFAULT 1,
    page_size INT DEFAULT 10
)
RETURNS JSONB
LANGUAGE plpgsql
AS $$
DECLARE
    result JSONB;
    total_count INT;
    offset_val INT := (page_number - 1) * page_size;
    total_pages INT;
BEGIN
    -- 1. Count total matching requests (before pagination)
    SELECT COUNT(*) INTO total_count
    FROM "SigningRequests" sr
    WHERE sr."RequesterId" = user_id
      AND (status_filter IS NULL OR sr."Status" = status_filter)
      AND (name_filter IS NULL OR sr."Title" ILIKE '%' || name_filter || '%');

    total_pages := CEIL(total_count::decimal / page_size)::int;

    -- 2. Fetch paginated data with metadata embedded into each request object
    SELECT jsonb_agg(requests_with_recipients) INTO result
    FROM (
        SELECT
            sr."Id" AS "requestId",
            sr."Title",
            sr."CreatedAt",
            sr."Status",
            (
                SELECT jsonb_agg(
                    jsonb_build_object(
                        'name', r."RecipientName",
                        'email', r."RecipientEmail",
                        'phone', r."RecipientPhone",
                        'action', r."Status",
                        'signed', CASE WHEN r."Status" = 'Completed' THEN true ELSE false END,
                        'rejected', CASE WHEN r."Status" = 'Rejected' THEN true ELSE false END,
                        'order', r."SigningOrder",
                        'positions', (
                            SELECT jsonb_agg(
                                jsonb_build_object(
                                    'page', sp."PageNumber",
                                    'x', sp."XPosition",
                                    'y', sp."YPosition"
                                )
                            )
                            FROM "SignaturePositions" sp
                            WHERE sp."RecipientId" = r."Id"
                        )
                    )
                )
                FROM "SigningRecipients" r
                WHERE r."RequestId" = sr."Id"
                AND (name_filter IS NULL OR r."RecipientName" ILIKE '%' || name_filter || '%' OR r."RecipientEmail" ILIKE '%' || name_filter || '%')
            ) AS recipients,
            total_count AS "totalCount",
            page_size AS "pageSize",
            page_number AS "currentPage",
            total_pages AS "totalPages"
        FROM "SigningRequests" sr
        WHERE sr."RequesterId" = user_id
          AND (status_filter IS NULL OR sr."Status" = status_filter)
          AND (name_filter IS NULL OR sr."Title" ILIKE '%' || name_filter || '%')
        ORDER BY sr."CreatedAt" DESC
        LIMIT page_size OFFSET offset_val
    ) AS requests_with_recipients;

    RETURN COALESCE(result, '[]'::jsonb);
END;
$$;


-- ===========================================================================================
-- END OF FUNCTION
-- ===========================================================================================
