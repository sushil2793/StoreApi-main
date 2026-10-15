using SmartMeterAdmin.Data.HelpEntity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Services.Help
{
    public interface IHelpDocument
    {

        Task<IList<HelpDocument>> GetAllHelpDocument();


        Task<HelpDocument> GetHelpDocumentById(int id);

        Task Create(HelpDocument data);
        Task Update(HelpDocument data);


    }
}
