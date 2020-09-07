using System.Threading.Tasks;

namespace WhyNotEarth.Meredith.BrowTricks.Jobs
{
    public interface IClientSaveSignatureJob
    {
        Task SaveSignature(int formSignatureId);
    }
}