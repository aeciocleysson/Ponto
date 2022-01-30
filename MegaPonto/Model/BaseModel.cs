using System;
using System.ComponentModel.DataAnnotations;

namespace MegaPonto.Model
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; private set; }

        public DateTime Inserted { get; private set; }

        public DateTime? UpdateAt { get; set; }
        public bool IsDelete { get; set; }

        public BaseModel()
        {
            Inserted = DateTime.Today;
            IsDelete = false;
        }
    }
}
