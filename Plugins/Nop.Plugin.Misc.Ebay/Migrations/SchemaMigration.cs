using System;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Ebay.Domains;

namespace Nop.Plugin.Misc.Ebay.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/09/14 09:30:17:6455422", "Ebay Marketplace Schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {

            if (!Schema.Schema("dbo").Table("DispatchedOrder").Exists())
                _migrationManager.BuildTable<DispatchedOrder>(base.Create);
            
            if (!Schema.Schema("dbo").Table("EbayClients").Exists())
                _migrationManager.BuildTable<EbayClient>(base.Create);

            if (!Schema.Schema("dbo").Table("EbayConfiguration").Exists())
                _migrationManager.BuildTable<EbayConfiguration>(base.Create);

            if (!Schema.Schema("dbo").Table("DeliveryLabel").Exists())
                _migrationManager.BuildTable<DeliveryLabel>(base.Create);

            if (!Schema.Schema("dbo").Table("EbayDispatchableOrders").Exists())
                _migrationManager.BuildTable<EbayDispatchableOrders>(base.Create);

            AddNewSchemaColumns();
        }

        private void AddNewSchemaColumns()
        {
            if (!Schema.Schema("dbo").Table("DispatchedOrder").Column("DispatchRequestedBy").Exists())
                Create.Column("DispatchRequestedBy").OnTable("DispatchedOrder").AsString(256).WithDefaultValue(null);

            if (!Schema.Schema("dbo").Table("DispatchedOrder").Column("DispatchRequestedOn").Exists())
                Create.Column("DispatchRequestedOn").OnTable("DispatchedOrder").AsDateTime2().WithDefaultValue(null);
        }
    }
     [SkipMigrationOnUpdate]
    [NopMigration("2020/05/17 09:30:14:6455422", "Initial Data")]
    public class InitialData : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public InitialData(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (Schema.Schema("dbo").Table("EbayConfiguration").Exists())
            {
                Insert.IntoTable("EbayConfiguration").Row(
                    new
                    {
                        StoreId = "1",
                        IsSandBox = "0",
                        IsActive = "1",
                        DevId = "fbfbd66c-8c30-4b71-8b9b-84d411e2dd6b",
                        AppId = "ArfanGul-iConnect-PRD-2c8ee5576-6929a962",
                        CertId = "PRD-c8ee5576e99b-6136-406a-969c-004a",
                        Version = "1068",
                        SignInUrl = "https://signin.ebay.com/ws/eBayISAPI.dll?SignIn&runame={0}&SessID={1}",
                        Endpoint = "nothing",
                        SiteCode = "3",
                        RuName = "Arfan_Gull-ArfanGul-iConne-muztqn"
                    });

                Insert.IntoTable("EbayClients").Row(
                    new
                    {
                        ConfigurationId = 1,
                        Token =
                            "AgAAAA**AQAAAA**aAAAAA**FfNHXw**nY+sHZ2PrBmdj6wVnY+sEZ2PrA2dj6AMlIuoCJOApA+dj6x9nY+seQ**eGQGAA**AAMAAA**66Pgis3c1G4V49YU4IQFzSwmWcJWf2pxFbN0nU1GOizVfbhF8VP+CLDX8NN+AH8njayl7ommCdfp5rtG/D0RhkanTQfcWo0mgRmPI7XU5o431QTDG8+JK76ElYqW8vlvTBvEUMXOOJ/4UcZmetufIRX1B5Q7MocucFXf43FAJbSn11SdKoPOYCFG8oEJiT/gbPKcAHh7tTefepldmblG2zPo0wMN3uA2dZGJj0IpoHmcGU6X3hdx9DPvQ1wtcQ4ZpSAuj9ou+wrvXWLoWcYa70HzWQPcw0zwXTmi353H/7+TuEKsT1T+GVU8wnCEIy5Lt++BcwgKKxo9dljUvrNv6m1FRVjSqWHWDVcB3CMHo4R6NKdgfX9Jd2rNnnwms4Dauz9GSz6QfiqEPxgTEwYCKj11bCEYDfEGVpdQEQRehxsddLUoMcGycXDuGW6baBkMZB/4JAc4IR3BdZanOKUDtbtCD3AJjlxj6VgGOAGtzk9IW5jAKDK3514DJhFNG1pMHZF8UQTzAeozyxfGlxl5HVamu8AoTSH3IEJ8hfBKGVpfDZRV8ioiZu6C6s/zhNr6VCSrJ06ocoV7npcHGEv/dJHGkcaSgMX5h6WIrNevUlz6LJvEFYb1uNaDTSb7CNxyai0KxstRZfbDdJu6jK2WaZBEep+aguiEN4A6U46cpScuQlSttGJW6Omy5edXAG5M8Q7K295S8aIjOXv9smybDl2W9FKDbMVoAa44VEDfSfWdLYADTPojPorYkNtfXX5A",
                        UserName = "ppsc2018",
                        TokenExpiresOn = DateTime.Now.AddDays(365),
                        LastImportTime = DateTime.Now,
                        Comments = "Imported from Ebay",
                        IsActive = "1"
                    });
            }
        }
    }
}
