using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoppleWebDemo.Models
{
    [Table("feedback_table")]
    public class Feedback
    {
        [Key]
        public int Index { get; set; }
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}