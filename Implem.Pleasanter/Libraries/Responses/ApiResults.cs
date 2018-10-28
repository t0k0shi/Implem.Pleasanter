﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResults
    {
        public static ContentResult Success(long id, string message)
        {
            return Get(ApiResponses.Success(id, message));
        }

        public static ContentResult Error(Context context, Error.Types type, params string[] data)
        {
            return Get(ApiResponses.Error(
                context: context,
                type: type,
                data: data));
        }

        public static ContentResult Get(ApiResponse apiResponse)
        {
            return Get(apiResponse.ToJson());
        }

        public static ContentResult Get(string apiResponse)
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = apiResponse
            };
        }

        public static ContentResult BadRequest(Context context)
        {
            return Get(ApiResponses.BadRequest(context: context));
        }

        public static ContentResult Unauthorized(Context context)
        {
            return Get(ApiResponses.Unauthorized(context: context));
        }
    }
}