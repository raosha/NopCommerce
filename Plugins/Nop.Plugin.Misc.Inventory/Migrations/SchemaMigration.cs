using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Inventory.Domains;

namespace Nop.Plugin.Misc.Inventory.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/09/15 09:30:17:6455422", "Inventory schema")]
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
            if (!Schema.Schema("dbo").Table("PurchaseOrder").Exists())
                _migrationManager.BuildTable<PurchaseOrder>(base.Create);

            if (!Schema.Schema("dbo").Table("PurchaseOrderLine").Exists())
                _migrationManager.BuildTable<PurchaseOrderLine>(base.Create);

            if (!Schema.Schema("dbo").Table("PurchaseOrderNote").Exists())
                _migrationManager.BuildTable<PurchaseOrderNote>(base.Create);
        }
    }
}
