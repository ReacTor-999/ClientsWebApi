using ClientsWebApi.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ClientsWebApi.Entities
{
    public static class DbContextExtensions
    {
        public static DbSavingResultDto TrySaveChanges(this DbContext dbContext)
        {
            try
            {
                int result = dbContext.SaveChanges();
                return new DbSavingResultDto()
                {
                    IsSuccess = true,
                    Message = $"Results are saved successfully ({result} rows affected)."
                };
            }
            catch (Exception ex)
            {
                return new DbSavingResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static async Task<DbSavingResultDto> TrySaveChangesAsync(this DbContext dbContext)
        {
            try
            {
                int result = await dbContext.SaveChangesAsync();
                return new DbSavingResultDto()
                {
                    IsSuccess = true,
                    Message = $"Results are saved successfully ({result} rows affected)."
                };
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();

                do
                {
                    sb.AppendLine(ex.Message);
                    ex = ex.InnerException!;
                } while (ex != null);

                return new DbSavingResultDto()
                {
                    IsSuccess = false,
                    Message = sb.ToString(),
                };
            }
        }
    }
}
