using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.DataBase;
using Server.Models;
using System.IO;

namespace Server.Controllers.CardsController
{
    [Route("api/cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        Base db;
        FileBase Fdb;
        int MAX_COUNT_CARDS_GET = 100;
        int DEFAULT_COUNT_CARDS_GET = 10;
        public CardsController(Base data, FileBase fileBase)
        {
            this.db = data;
            this.Fdb = fileBase;
        }
        
        // GET: api/cards
        [HttpGet]
        public JsonResult Get(string sort, int? offset, int? count)
        {
            if (sort == null) sort = "1";
            if (count == null || count < 1 || count > this.MAX_COUNT_CARDS_GET) count = this.DEFAULT_COUNT_CARDS_GET;
            if (offset == null || offset < 0) offset = 0;
            return new JsonResult(db.getCard((int)count, (int)offset, sort));
        }

        // GET api/cards/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try { return new JsonResult(db.getCardById(id)); }
            catch (Exception) { return new BadRequestResult();}
        }

        // POST api/cards
        [HttpPost]
        public IActionResult Post(ProductCard card)
        {
            if (card.ImageData != null)
            {
                try { 
                    card.ImageSrc = this.Fdb.createPicture(card.ImageData);
                    card.ImageData = null;
                    if (card.isValid() && this.db.createCard(card)) return new OkResult();
                }
                catch(Exception) { return new BadRequestResult(); }  
            }
            return new BadRequestResult();
        }

        // POST api/cards/update
        [HttpPost]
        [Route("update")]
        public IActionResult Update(ProductCard card)
        {
            if (card.ImageData != null)
            {
                try{
                    this.Fdb.updatePicture(card.ImageSrc, card.ImageData);
                    card.ImageData = null;
                }
                catch (Exception) { return new BadRequestResult(); }
            }
            if (card.isValid() && this.db.updateCard(card)) return new OkResult();
            else return new BadRequestResult();
        }

        // DELETE api/cards/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try {
                this.Fdb.deletePicture(this.db.getCardById(id).ImageSrc);
                return new JsonResult(db.deleteCardById(id)); }
            catch (Exception) { return new BadRequestResult(); }
        }
    }
}
