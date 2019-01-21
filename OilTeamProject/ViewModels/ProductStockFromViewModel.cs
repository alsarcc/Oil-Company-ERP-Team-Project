using Foolproof;
using OilTeamProject.Controllers;
using OilTeamProject.Models.Customers;
using OilTeamProject.Models.Factories;
using OilTeamProject.Models.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace OilTeamProject.ViewModels
{
    public class ProductStockFromViewModel
    {
        private string id;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id
        {
            get
            {
                return ProductID.ToString() + BottlingID.ToString();
            }
            set
            {
                id = value;
            }
        }

        public const int MinimumStock = 1000;
        public const int MaximumStock = 5000;
        public const int ReorderingLevel = 2500;

        public string Name { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        [Range(0, MaximumStock)]
        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }

        [Range(0, MaximumStock)]
        [GreaterThan("AvailableQuantity")]
        [Display(Name = "Actual Quantity")]
        public int ActualQuantity { get; set; }

        public bool IsLow { get; set; }

        public IEnumerable<Bottling> Bottlings { get; set; }
        public IEnumerable<Sector> Sectors { get; set; }
        public IEnumerable<Product> Products { get; set; }
        // FOREIGN KEYS
        public int SectorID { get; set; }

        public Bottling Bottling { get; set; }
        public int BottlingID { get; set; }

        public int ProductID { get; set; }

        public IEnumerable<OrderProducts> OrderProducts { get; set; }
        
        public DateTime CalculateExpirationDate()
        {
            ExpirationDate = Bottling.BottlingDate.AddYears(1);

            return ExpirationDate;
        }

        public bool CheckQuantity()
        {
            if (AvailableQuantity <= MinimumStock)
                return IsLow = true;
            else
                return IsLow = false;

        }
    }
}