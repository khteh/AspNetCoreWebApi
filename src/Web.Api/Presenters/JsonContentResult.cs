using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
namespace Web.Api.Presenters;

public sealed class JsonContentResult : ContentResult
{
    public JsonContentResult() => ContentType = Application.Json;
}