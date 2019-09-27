using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using System.Linq;
namespace Web.Api.Presenters.Grpc
{
    public class ChangePasswordPresenter : IOutputPort<UseCaseResponseMessage>
    {
        public Web.Api.Core.Grpc.Response Response {get; private set;}
        public void Handle(UseCaseResponseMessage response)
        {
            Response = new Web.Api.Core.Grpc.Response();
            Response.Success = response.Success;
            if (response.Errors != null && response.Errors.Any())
                foreach (Error error in response.Errors)
                    Response.Errors.Add(new Web.Api.Core.Grpc.Error() {Code = error.Code, Description = error.Description});
        }
    }
}