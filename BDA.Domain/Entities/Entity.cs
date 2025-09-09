using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BDA.Entities
{
    public abstract class Entity<T>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public T Id { get; set; }

        private DateTime createdOn = DateTime.Now;
        public DateTime CreatedOn { get => createdOn; set => createdOn = value; }

        private DateTime updatedOn = DateTime.Now;
        public DateTime UpdatedOn { get => updatedOn; set => updatedOn = value; }

        private bool isActive = true;
        public bool IsActive { get => isActive; set => isActive = value; }
    }
}
