using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ProductCard
    {
        public int ID { get; set; }
        public string name { get; set;}
        public string ImageSrc { get; set; }
        public string ImageData { get; set; }

        public ProductCard(){}

        public void update(ProductCard card)
        {
            this.name = card.name;
            //this.ImageSrc = card.ImageSrc;
        }

        public bool isValid()
        {
            return name != null && name.Length > 3;
        }
    }
}
