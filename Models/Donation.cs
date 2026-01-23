using System;
using System.ComponentModel.DataAnnotations;

namespace DonationsApp.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        [Required]
        public decimal Amount { get; set; }  // مبلغ التبرع

        [Required]
        [StringLength(100)]
        public required string DonorName { get; set; }  // اسم المتبرع

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;  // تاريخ التبرع

        public required string UserId { get; set; }  // Foreign Key للمستخدم
    }
}
