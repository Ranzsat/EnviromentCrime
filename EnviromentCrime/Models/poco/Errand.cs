using EnviromentCrime.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
    public class Errand
    {
        public int ErrandID { set; get; }
        public string RefNumber { set; get; }

        [Display(Name = "Var har brottet skett någonstans?")]
        [Required(ErrorMessage ="Du måste fylla i platsen för bråttet")]
        public string Place { set; get; }

        [Display(Name = "Vilken typ av brott?")]
        [Required(ErrorMessage ="Du måste fylla i vilket datum det är")]
        public string TypeOfCrime { set; get; }

        [Display(Name = "När skedde brottet?")]
        [Required(ErrorMessage ="Du måste fylla i vilket datum det är")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfObservation { set; get; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Beskriv din observation (ex. namn på misstänkt person):")]
        public string Observation { set; get; }
        public string InvestigatorInfo { set; get; }
        public string InvestigatorAction { set; get; }

        [Display(Name = "Ditt namn (för- och efternamn):")]
        [Required(ErrorMessage ="Du måste fylla i ditt namn")]
        public string InformerName { set; get; }

        [Phone]
        [Display(Name = "Din telefon:")]
        [Required(ErrorMessage ="Du måste fylla i ditt telefonnummer")]
        [RegularExpression(@"^(07[02369])\s*(\d{4})\s*(\d{3})$", 
            ErrorMessage ="Formatet stämmer inte, det är 07 följt utav resterande siffror")]
        public string InformerPhone { set; get; }

        public string StatusId { set; get; }
        public string DepartmentId { set; get; }
        public string EmployeeId { set; get; }

        public ICollection<Picture> Pictures { get; set; }
        public ICollection<Sample> Samples { get; set; }
    }
}
