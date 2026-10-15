using SmartMeterAdmin.Infrastructure.Data;
using System.Threading.Tasks;
using SmartMeterAdmin.Data.HelpEntity;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SmartMeterAdmin.Services.Help
{
    public class HelpDocumentservice : IHelpDocument
    {
        private readonly SmartMeterContext _context;
        public HelpDocumentservice(SmartMeterContext context)
        {
            _context = context;
        }
        public async Task Create(HelpDocument data)
        {

            await _context.HelpDocuments.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<HelpDocument>> GetAllHelpDocument()
        {
            return await _context.HelpDocuments.Where(a => a.IsDelete == false).ToListAsync();
        }

        public async Task<HelpDocument> GetHelpDocumentById(int id)
        {
            return await _context.HelpDocuments.FindAsync(id);
        }

        public async Task Update(HelpDocument data)
        {
            _context.HelpDocuments.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}
