using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using me.bellacall.Core.Locales;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code>Кампании</code>")]
    public class CampaignsController : ApiController<Campaign, CampaignModel>
    {
        public CampaignsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Campaign GetEntity(CampaignModel model)
        {
            return new Campaign
            {
                Id = model.Id,
                Company_Id = model.Company_Id,
                Name = model.Name,
                Tariff_Id = model.Tariff_Id
            };
        }

        protected override CampaignModel GetModel(Campaign entity)
        {
            return new CampaignModel
            {
                Id = entity.Id,
                Company_Id = entity.Company_Id,
                Name = entity.Name,
                Tariff_Id = entity.Tariff_Id
            };
        }

        /// <summary>
        /// Возвращает список кампаний
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Campaigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampaignModel>>> GetCampaigns()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => e.Company_Id == COMPANY_ID)
                .Join(AllowedIds(Operation.Read), o => o.Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает кампанию
        /// </summary>
        /// <param name="id">ID кампании</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Campaigns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignModel>> GetCampaign(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Where(e => e.Company_Id == COMPANY_ID)
                .Join(AllowedIds(Operation.Read), o => o.Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет кампанию
        /// </summary>
        /// <param name="id">ID кампании</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Campaigns/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(long id, CampaignModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign_Id = model.Id;

            var result = Check(model.Company_Id == COMPANY_ID, Forbidden).OkNull() ?? Check(Operation.Update, campaign_Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign_Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет кампанию
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Campaigns
        [HttpPost]
        public async Task<ActionResult<CampaignModel>> PostCampaign(CampaignCreateModel model)
        {
            var result = Check(model.Company_Id == COMPANY_ID, Forbidden).OkNull() ?? Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, entity.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetCampaign), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет кампанию
        /// </summary>
        /// <param name="id">ID кампании</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Campaigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(long id)
        {
            var entity = await DB_TABLE
                .Include(e => e.ContactGroups)  // stop
                .Include(e => e.Jobs)           // stop
                .Include(e => e.Leads)          // stop
                .Include(e => e.Scripts)        // stop
                .Include(e => e.Gateways)
                .ThenInclude(e => e.GatewayStreams)
                .Include(e => e.ContactProperties)
                .Include(e => e.LeadProperties)
                .Include(e => e.UserRoles)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null) return NotFound();

            var campaign_Id = id;

            var result =
                Check(entity.Company_Id == COMPANY_ID, Forbidden).OkNull() ??
                Check(Operation.Delete, campaign_Id).OkNull() ??
                Check(entity.ContactGroups.Count == 0, Forbidden, string.Format(Strings.Cascade_Message, Strings.Campaign_Entity, Strings.ContactGroup_List)).OkNull() ??
                Check(entity.Jobs.Count == 0, Forbidden, string.Format(Strings.Cascade_Message, Strings.Campaign_Entity, Strings.Job_List)).OkNull() ??
                Check(entity.Leads.Count == 0, Forbidden, string.Format(Strings.Cascade_Message, Strings.Campaign_Entity, Strings.Lead_List)).OkNull() ??
                Check(entity.Scripts.Count == 0, Forbidden, string.Format(Strings.Cascade_Message, Strings.Campaign_Entity, Strings.Script_List));

            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.Gateways.SelectMany(e => e.GatewayStreams));
                DB.RemoveRange(entity.Gateways);
                DB.RemoveRange(entity.ContactProperties);
                DB.RemoveRange(entity.LeadProperties);
                DB.RemoveRange(entity.UserRoles);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign_Id, GetModel(entity));
                Get<GatewayStreamsController>().Log(Operation.Delete, operation_Id, campaign_Id, entity.Gateways.SelectMany(e => e.GatewayStreams));
                Get<GatewaysController>().Log(Operation.Delete, operation_Id, campaign_Id, entity.Gateways);
                Get<ContactPropertiesController>().Log(Operation.Delete, operation_Id, campaign_Id, entity.ContactProperties);
                Get<LeadPropertiesController>().Log(Operation.Delete, operation_Id, campaign_Id, entity.LeadProperties);
                Get<AspNetUserRolesController>().Log(Operation.Delete, operation_Id, campaign_Id, entity.UserRoles);
            }

            Log(DB_TABLE.GetName(), Operation.Delete, campaign_Id, GetModel(entity));

            return NoContent();
        }
    }
}
