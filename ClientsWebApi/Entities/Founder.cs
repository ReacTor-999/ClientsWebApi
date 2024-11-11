using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClientsWebApi.Entities
{
    /* Прим. Валидаци ИНН основана определении ИНН в законодательстве РФ. 
     * См. Приказ ФНС России от 29.06.2012 N ММВ-7-6/435@ 
     * "Об утверждении Порядка и условий присвоения, применения, а также изменения идентификационного номера налогоплательщика" 
     * https://www.nalog.gov.ru/rn77/related_activities/accounting/recording_individuals/3970311/
     */

    public class Founder: EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Column(name: "TIN", TypeName = "VARCHAR"), MaxLength(12)]
        public string TaxpayerIndividualNumber { get; set; } = null!;

        [MaxLength(20)]
        public string FirstName { get; set; } = null!;

        [MaxLength(20)]
        public string LastName { get; set; } = null!;

        [MaxLength(20)]
        public string? Patronymic { get; set; }

        public string FullName { get; set; } = null!;

        public List<Client> Clients { get; set; } = null!;

    }
}
