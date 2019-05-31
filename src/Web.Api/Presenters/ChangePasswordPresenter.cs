using System.Net;
using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public class ChangePasswordPresenter : PresenterBase<ChangePasswordResponse>
    {
        public override void Handle(ChangePasswordResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success ? JsonSerializer.SerializeObject(response.Id) :
                                                        JsonSerializer.SerializeObject(response.Message);
        }
    }
}
