using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Server.DataBase
{
    public class Base
    {
        string JsonFile;
        static int MAX_ID;
        List<ProductCard> cards;

        public Base(string jsonFile)
        {
            this.cards = new List<ProductCard>(10);
            this.JsonFile = jsonFile;
            MAX_ID = 10;
            this.loadFromFile();      
        }

        async void loadFromFile()
        {
            if (File.Exists(this.JsonFile))
            {
                using (FileStream openStream = File.OpenRead(this.JsonFile))
                {
                    List<ProductCard> items = await JsonSerializer.DeserializeAsync<List<ProductCard>>(openStream);
                    
                    if (items != null) this.cards.AddRange(items);
                }
                this.setMaxId();
            }            
        }

        void setMaxId()
        {
            foreach (ProductCard card in this.cards)
                if (card.ID >= MAX_ID) MAX_ID = card.ID + 1;
        }

        async void dumpToFile()
        {
            await File.WriteAllBytesAsync(this.JsonFile, JsonSerializer.SerializeToUtf8Bytes(this.cards));
        }

        public ProductCard getCardById(int id){
            foreach (ProductCard card in this.cards)
                if (card.ID == id)
                    return card;
            throw new Exception("Not Found this ID");
        }

        public ProductCard[] getCard(int countMax, int offset, string sort) {
            if(sort == "name") cards.Sort(delegate (ProductCard a, ProductCard b) { return a.name.CompareTo(b.name); });
            if (sort == "id") cards.Sort(delegate (ProductCard a, ProductCard b) { return a.ID.CompareTo(b.ID); });

            if (cards.Count > (countMax + offset)) return cards.GetRange(offset, countMax).ToArray();
            else if(cards.Count > offset) return cards.GetRange(offset, cards.Count - offset).ToArray();
            else return null;
        }

        public bool createCard(ProductCard card){
            card.ID = MAX_ID++;
            this.cards.Add(card);
            Task.Run(dumpToFile);
            return true;
        }

        public bool deleteCardById(int id) {
            try
            {
                bool answer =  this.cards.Remove(this.getCardById(id));
                if(answer)Task.Run(dumpToFile);
                return answer;
            }
            catch (Exception) { return false; }
        }

        public bool updateCard(ProductCard newCard)
        {
            try
            {
                this.getCardById(newCard.ID).update(newCard);
                Task.Run(dumpToFile);
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
