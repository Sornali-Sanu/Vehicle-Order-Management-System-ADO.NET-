using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleOrderMasterDetail.ViewModel
{
    public class VehicleViewModel
    {
        public int SlNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrderNo { get; set; }
        public DateTime DateOfOrder { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public bool IsRegular { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public string Model { get; set; }
        public Decimal Price { get; set; }
    }
}
