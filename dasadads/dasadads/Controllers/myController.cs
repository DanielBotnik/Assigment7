using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common_MVC_Project.Models;
using System.IO;
using System.Net;
using System.Security.Policy;

namespace Common_MVC_Project.Controllers
{
    public class myController : Controller
    {
        static Random myRand = new Random();
        List<myModel> myListModel = new List<myModel>(16);
        private static JavaScriptSerializer myJavaScriptSerializer = new JavaScriptSerializer();
        private const string url = "https://localhost:44323/Handler1.ashx?action=";

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ShuffleAction()
        {
            await Task.Run(() => Shuffle());
            return Json(myListModel, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> NextStepAction(int index,int empty,string background,string button)
        {
            myModel res = await Task.Run(() => NextStep(index,empty,background,button));
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> IsGameOverAction(string data)
        {
            bool res = await Task.Run(() => IsGameOver(data));
            return Json(res,JsonRequestBehavior.AllowGet);
        }
        public void Shuffle()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(url + "Shuffle");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            myModel[] res = (myModel[])myJavaScriptSerializer.Deserialize(json, typeof(myModel[]));
            for (int i = 0; i < 16; i++)
                myListModel.Insert(i, res[i]);
        }

        public myModel NextStep(int index,int empty,string background,string button)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{url}NextStep&emptyIndex={empty}&clickedIndex={index}&background={background}&current={button}");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            return (myModel)myJavaScriptSerializer.Deserialize(json, typeof(myModel));
        }

        public bool IsGameOver(string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{url}IsGameOver&data={data}");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            return json == "\"true\"";
        }
    }
}