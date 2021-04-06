using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Server
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : HttpTaskAsyncHandler
    {
        private static Random random = new Random();
        private static JavaScriptSerializer myJavaScriptSerializer = new JavaScriptSerializer();
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            string result = "";
            if  (context.Request.QueryString["action"] == "IsGameOver")
                result = await Task<string>.Run(() => IsGameOver(context));
            else if (context.Request.QueryString["action"] == "Shuffle")
                result = await Task<string>.Run(() => Shuffle(context));
            else if (context.Request.QueryString["action"] == "NextStep")
                result = await Task<string>.Run(() => NextStep(context));
            context.Response.Write(result);
        }

        private string IsGameOver(HttpContext context)
        {
            string jsonData = context.Request.QueryString["data"];
            TextAndColor[] board = (TextAndColor[])myJavaScriptSerializer.Deserialize(jsonData, typeof(TextAndColor[]));
            for (int i = 0; i < 2; i++)
                if (board[i].text != (i + 1).ToString())
                    return myJavaScriptSerializer.Serialize("false");
            return myJavaScriptSerializer.Serialize("true");
        }

        private string Shuffle(HttpContext context)
        {
            List<int> rndInts = new List<int>();
            for (int i = 1; i < 16; i++)
                rndInts.Add(i);
            TextAndColor[] board = new TextAndColor[16];
            for(int i = 0; i < 15; i++)
            {
                int rndInt = rndInts[random.Next(rndInts.Count)];
                board[i] = new TextAndColor();
                board[i].R = random.Next(256);
                board[i].G = random.Next(256);
                board[i].B = random.Next(256);
                board[i].text = rndInt.ToString();
                rndInts.Remove(rndInt);
            }
            board[15] = new TextAndColor();
            board[15].R = 255;
            board[15].G = 255;
            board[15].B = 255;
            board[15].text = "";
            return myJavaScriptSerializer.Serialize(board);
        }

        private string NextStep(HttpContext context)
        {
            int emptyIndex = int.Parse(context.Request.QueryString["emptyIndex"]);
            int clickedIndex = int.Parse(context.Request.QueryString["clickedIndex"]);
            string jsonData1 = context.Request.QueryString["background"];
            string jsonData2 = context.Request.QueryString["current"];
            TextAndColor background = (TextAndColor)myJavaScriptSerializer.Deserialize(jsonData1, typeof(TextAndColor));
            TextAndColor current = (TextAndColor)myJavaScriptSerializer.Deserialize(jsonData2, typeof(TextAndColor));
            TextAndColor toReturn = new TextAndColor();
            if ((Math.Abs(emptyIndex - clickedIndex) == 1 && emptyIndex / 4 == clickedIndex / 4) || (Math.Abs(emptyIndex - clickedIndex) == 4))
            {
                toReturn.R = (background.R + current.R) / 2;
                toReturn.G = (background.G + current.G) / 2;
                toReturn.B = (background.B + current.B) / 2;
                toReturn.text = "true";
            }
            else
                toReturn.text = "false";
            return myJavaScriptSerializer.Serialize(toReturn);
        }
        
        public override bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}