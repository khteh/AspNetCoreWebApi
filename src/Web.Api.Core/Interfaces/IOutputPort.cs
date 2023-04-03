using System.Threading.Tasks;

namespace Web.Api.Core.Interfaces;
public interface IOutputPort<in TUseCaseResponse>
{
    Task Handle(TUseCaseResponse response);
}