using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using me.bellacall.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code>Кампании &rarr; Шлюзы &rarr; Потоки</code>")]
    public class GatewayStreamsController : ApiController<GatewayStream, GatewayStreamModel>
    {
        public GatewayStreamsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override GatewayStream GetEntity(GatewayStreamModel model)
        {
            return new GatewayStream
            {
                Id = model.Id,
                Gateway_Id = model.Gateway_Id,
                Name = model.Name,
                UserName = model.UserName,
                Password = model.Password,
                TrunkType = model.TrunkType,
                TrunkCount = model.TrunkCount
            };
        }

        protected override GatewayStreamModel GetModel(GatewayStream entity)
        {
            return new GatewayStreamModel
            {
                Id = entity.Id,
                Gateway_Id = entity.Gateway_Id,
                Name = entity.Name,
                UserName = entity.UserName,
                Password = entity.Password,
                TrunkType = entity.TrunkType,
                TrunkCount = entity.TrunkCount
            };
        }

        /// <summary>
        /// Возвращает список потоков
        /// </summary>
        /// <param name="gateway_Id">GATEWAY_ID потока</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/GatewayStreams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GatewayStreamModel>>> GetGatewayStreams([FromQuery(Name = "oid")] long[] gateway_Id)
        {
            var result = Check(DB.Gateways, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => gateway_Id.Contains(e.Gateway_Id))
                .Join(AllowedIds(Operation.Read), o => o.Gateway.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает поток
        /// </summary>
        /// <param name="id">ID потока</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/GatewayStreams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GatewayStreamModel>> GetGatewayStream(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(DB.Gateways, Operation.Read), o => o.Gateway.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет поток
        /// </summary>
        /// <param name="id">ID потока</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/GatewayStreams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGatewayStream(long id, GatewayStreamModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Gateways.Find(model.Gateway_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Gateways, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет поток
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/GatewayStreams
        [HttpPost]
        public async Task<ActionResult<GatewayStreamModel>> PostGatewayStream(GatewayStreamCreateModel model)
        {
            var campaign = DB.Gateways.Find(model.Gateway_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Gateways, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetGatewayStream), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет поток
        /// </summary>
        /// <param name="id">ID потока</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/GatewayStreams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGatewayStream(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.Gateway?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Gateways, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
