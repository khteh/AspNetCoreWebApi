using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Presenters.Grpc
{
    public class DeleteUserPresenter : IOutputPort<UseCaseResponseMessage>
    {
        public Web.Api.Core.Accounts.DeleteUserResponse Response {get; private set;}
        public void Handle(UseCaseResponseMessage response)
        {
            Response = new Web.Api.Core.Accounts.DeleteUserResponse();
            Response.Id = response.Id;
            Response.Response = new Web.Api.Core.Grpc.Response();
            Response.Response.Success = response.Success;
            if (response.Errors != null && response.Errors.Any())
                foreach (Error error in response.Errors)
                    Response.Response.Errors.Add(new Web.Api.Core.Grpc.Error() {Code = error.Code, Description = error.Description});
        }
    }
}