using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientsWebApi.Entities
{
    /* Прим. Валидаци ИНН основана определении ИНН в законодательстве РФ. 
     * См. Приказ ФНС России от 29.06.2012 N ММВ-7-6/435@ 
     * "Об утверждении Порядка и условий присвоения, применения, а также изменения идентификационного номера налогоплательщика" 
     * https://www.nalog.gov.ru/rn77/related_activities/accounting/recording_individuals/3970311/
     */

    public class Client : EntityBase
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(12)]
        [Column(name: "TIN", TypeName = "VARCHAR")]
        public string TaxpayerIndividualNumber { get; set; } = null!;

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(3)] //именно 3, а не 2 - ИП, ЮЛ + возможное обозначение самозанятых как ИТД - индивидуальная трудовая деятельность
        public string Type { get; set; } = null!;


        public string? AccountId { get; set; } = null!;


        [ForeignKey(nameof(AccountId))]
        public IdentityUser? Account { get; set; }


        public List<Founder> Founders { get; set; } = null!;

    }
}
