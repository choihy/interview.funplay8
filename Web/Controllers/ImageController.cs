using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Http;
using System.Linq;
using Web.Models;
using System;
using System.Net;
using System.Net.Http;
using Web.DataAccess;

namespace Web.Controllers
{
    [RoutePrefix("api/image")]
    public class ImageController : ApiController
    {
        private int pageSize = Int32.Parse(ConfigurationManager.AppSettings["PageSize"]);

        [HttpGet]
        [Route("all")]
        public IEnumerable<dynamic> All()
        {
            using (var ctx = new DB_Entities())
            {
                return ctx.Images.Select(x => new
                {
                    x.ID,
                    x.Url,
                    x.Name,
                    x.RequestCount,
                    Page = ((x.ID / pageSize) + 1),
                }).ToList();
            }
        }

        [HttpGet]
        [Route("id/{id}")]
        public async Task<object> Id(int id)
        {
            using (var ctx = new DB_Entities())
            {
                var image = ctx.Images.SingleOrDefault(x => x.ID == id);
                if (image == null) return null;

                image.RequestCount++;
                await ctx.SaveChangesAsync();

                return new
                {
                    image.ID,
                    image.Url,
                    image.Name,
                    image.RequestCount,
                    Page = ((image.ID / pageSize) + 1),
                };
            }
        }

        [HttpGet]
        [Route("page/{page}")]
        public IEnumerable<dynamic> Page(int page)
        {
            var startId = pageSize * (page - 1);
            var endId = pageSize * page;

            using (var ctx = new DB_Entities())
            {
                return ctx.Images
                    .Where(x => x.ID > startId && x.ID <= endId)
                    .Select(x => new
                    {
                        x.ID,
                        x.Url,
                        x.Name,
                        x.RequestCount,
                        Page = ((x.ID / pageSize) + 1),
                    }).ToList();
            }
        }

        [HttpGet]
        [Route("popular")]
        public IEnumerable<dynamic> Popular()
        {
            var list = new List<dynamic>();
            using (var ctx = new DB_Entities())
            {
                var image = ctx.Images
                    .Where(x => x.RequestCount > 0)
                    .Select(x => new
                    {
                        x.ID,
                        x.Url,
                        x.Name,
                        x.RequestCount,
                        Page = ((x.ID / pageSize) + 1),
                    })
                    .OrderByDescending(x => x.RequestCount)
                    .FirstOrDefault();
                return new List<dynamic>() { image };
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create(ImagesModel model)
        {
            using (var ctx = new DB_Entities())
            {
                ctx.Images.AddRange(model.data.Select(x => new Image
                {
                    Name = x.Name,
                    Url = x.Url,
                }));

                await ctx.SaveChangesAsync();
            }

            return Ok();
        }

    }
}



