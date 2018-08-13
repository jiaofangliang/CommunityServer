/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;

namespace ASC.Projects.Data.DAO
{
    class ReportDao : BaseDao, IReportDao
    {
        private readonly string[] columns = new[] { "id", "type", "name", "filter", "cron", "create_by", "create_on", "tenant_id", "auto" };
        private readonly Converter<object[], ReportTemplate> converter;

        public ReportDao(int tenantID)
            : base(tenantID)
        {
            converter = ToTemplate;
        }


        public List<ReportTemplate> GetTemplates(Guid userId)
        {
            return Db.ExecuteList(Query(ReportTable)
                                    .Select(columns)
                                    .Where("create_by", userId.ToString())
                                    .OrderBy("name", true))
                .ConvertAll(converter);
        }

        public List<ReportTemplate> GetAutoTemplates()
        {
            return Db.ExecuteList(new SqlQuery(ReportTable).Select(columns)
                                                        .Where("auto", true)
                                                        .OrderBy("tenant_id", true))
                .ConvertAll(converter);
        }

        public ReportTemplate GetTemplate(int id)
        {
            return Db.ExecuteList(Query(ReportTable).Select(columns).Where("id", id)).ConvertAll(converter).SingleOrDefault();
        }

        public ReportTemplate SaveTemplate(ReportTemplate template)
        {
            if (template == null) throw new ArgumentNullException("template");
            if (template.CreateOn == default(DateTime)) template.CreateOn = DateTime.Now;
            if (template.CreateBy.Equals(Guid.Empty)) template.CreateBy = CurrentUserID;

            var insert = new SqlInsert(ReportTable, true)
                .InColumns(columns)
                .Values(
                    template.Id,
                    template.ReportType,
                    template.Name,
                    template.Filter.ToXml(),
                    template.Cron,
                    template.CreateBy.ToString(),
                    TenantUtil.DateTimeToUtc(template.CreateOn),
                    Tenant,
                    template.AutoGenerated)
                .Identity(0, 0, true);
            template.Id = Db.ExecuteScalar<int>(insert);
            return template;
        }

        public void DeleteTemplate(int id)
        {
            Db.ExecuteNonQuery(Delete(ReportTable).Where("id", id));
        }

        private static ReportTemplate ToTemplate(IList<object> r)
        {
            var tenant = CoreContext.TenantManager.GetTenant(Convert.ToInt32(r[7]));
            var template = new ReportTemplate((ReportType)Convert.ToInt32(r[1]))
            {
                Id = Convert.ToInt32(r[0]),
                Name = (string)r[2],
                Filter = r[3] != null ? TaskFilter.FromXml((string)r[3]) : new TaskFilter(),
                Cron = (string)r[4],
                CreateBy = ToGuid(r[5]),
                CreateOn = TenantUtil.DateTimeFromUtc(tenant.TimeZone, Convert.ToDateTime(r[6])),
                Tenant = tenant.TenantId,
                AutoGenerated = Convert.ToBoolean(r[8]),
            };
            return template;
        }
    }
}
