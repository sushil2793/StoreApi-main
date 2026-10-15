using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;
using SmartMeterAdmin.Data.Master;
using SmartMeterAdmin.Data.AssignMeter;
using SmartMeterAdmin.Data.CustomerData;
using SmartMeterAdmin.Data.IncomingMeter;
using SmartMeterAdmin.Data.NewConsumer;
using SmartMeterAdmin.Data.NewMeter;
using SmartMeterAdmin.Data.OldMeter;
using SmartMeterAdmin.Data.SimData;
using SmartMeterAdmin.Data.VillageData;
using SmartMeterAdmin.ViewModel.DropDown;
using SmartMeterAdmin.ViewModel.Master;
using SmartMeterAdmin.ViewModel.MeterViewModel;
using SmartMeterAdmin.ViewModel.UserViewModel;
using SmartMeterAdmin.ViewModel.SimDataView;
using SmartMeterAdmin.Data.HelpEntity;
using SmartMeterAdmin.Data.User;




namespace SmartMeterAdmin.Infrastructure.Data
{
    public class SmartMeterContext : DbContext
    {
        public SmartMeterContext(DbContextOptions<SmartMeterContext> options) : base(options)
        {
        }

        // DbSet for your entities
        public DbSet<Users> Users { get; set; }
        public DbSet<NewMeter> NewMeters { get; set; }
        public DbSet<OldMeter> OldMeters { get; set; }
        public DbSet<HelpDocument> HelpDocuments { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<Substation> Substations { get; set; }
        public DbSet<Feeder> Feeders { get; set; }
        public DbSet<DT> DTs { get; set; }
        public DbSet<CustomerData> CustomerData { get; set; }
        public DbSet<SimData> SimDatas { get; set; }
        public DbSet<AssignMeter> AssignMeters { get; set; }
        public DbSet<IncomingMeter> IncomingMeters { get; set; }
        public DbSet<NewConsumer> NewConsumers { get; set; }
        public DbSet<VillageData> VillageDatas { get; set; }
        #region Views
        public DbQuery<SimDataView> SimDataViews { get; set; }
        public DbQuery<PostDeviceViewModel> Getpostdevice { get; set; }
        public DbQuery<ConsumerMeterDetailsViewModel> ConsumerMeters { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure your entities
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<NewMeter>().ToTable("NewMeter");
            modelBuilder.Entity<OldMeter>().ToTable("OldMeter");
            modelBuilder.Entity<HelpDocument>().ToTable("HelpDocument");

            modelBuilder.Entity<CustomerData>().ToTable("CustomerData");
            modelBuilder.Entity<SimData>().ToTable("SimData");
            modelBuilder.Entity<AssignMeter>().ToTable("AssignMeter");
            modelBuilder.Entity<IncomingMeter>().ToTable("IncomingMeter");
            modelBuilder.Entity<NewConsumer>().ToTable("NewConsumer");
            modelBuilder.Entity<VillageData>().ToTable("VillageData");

            #region Procedure

            modelBuilder.Entity<MeterCountViewModel>().HasNoKey().ToView("MeterCountView");
            modelBuilder.Entity<DropDown>().HasNoKey().ToView("SubDivisionDropdowns");
            modelBuilder.Entity<SyncData>().HasNoKey().ToView("SyncDatas");
            modelBuilder.Entity<DashboardViewModels>().HasNoKey().ToView("GetDashboardViewModels");
            modelBuilder.Entity<StoreDashBoardViewModel>().HasNoKey().ToView("GetStoreDashBoardViewModel");
            modelBuilder.Entity<UserViewModel>().HasNoKey().ToView("GetUsers");
            modelBuilder.Entity<AssignMeterForSingle>().HasNoKey().ToView("assignMeterFors");
            modelBuilder.Entity<IncomingMeterPrint>().HasNoKey().ToView("incomingMeterPrints");
            modelBuilder.Entity<PostDeviceMeterViewModel>().HasNoKey().ToView("newMeterViews");
            modelBuilder.Entity<NewMeterProc>().HasNoKey().ToView("meterProcs");
            modelBuilder.Entity<CheckNewMeterID>().HasNoKey().ToView("meterIDs");
            modelBuilder.Entity<ValidateMeter>().HasNoKey().ToView("ValidateMeters");
            modelBuilder.Entity<MeterListViewModel>().HasNoKey().ToView("meterListViews");
            modelBuilder.Entity<MeterCountFunc>().HasNoKey().ToView("MeterCountFunc");
            modelBuilder.Entity<NewMeterReport>().HasNoKey().ToView("NewMeterReport");
            modelBuilder.Entity<OldMeterReport>().HasNoKey().ToView("OldMeterReport");
            modelBuilder.Entity<AddDeviceApiModel>().HasNoKey().ToView("addDeviceApis");
            modelBuilder.Entity<AssignMeterForSingle>().HasNoKey().ToView("assignMeterFors");
            modelBuilder.Entity<CheckNewMeterID>().HasNoKey().ToView("meterIDs");
            modelBuilder.Entity<ConsumerMeterDetailsViewModel>().HasNoKey().ToView("ConsumerMeterDetailsViewModel");
            modelBuilder.Entity<ConsumerMeterDetailsViewModel>().HasNoKey().ToView("ConsumerMeterDetailsViewModel");
            modelBuilder.Entity<PostDeviceViewModel>().HasNoKey().ToView("PostDeviceViewModel");
            modelBuilder.Entity<SimDataView>().HasNoKey().ToView("SimDataView");

            #endregion

            #region SQL Function

            modelBuilder.Entity<MeterCountFunc>().ToView("MeterCountFunc");


            #endregion

            #region Views

            modelBuilder.Entity<SimDataView>().ToView("SimDataView");
            modelBuilder.Entity<NewMeterReport>().ToView("GetNewMeterView");
            modelBuilder.Entity<OldMeterReport>().ToView("GetOldMeters");
            modelBuilder.Entity<PostDeviceViewModel>().ToView("getpostdevice");
            modelBuilder.Entity<ConsumerMeterDetailsViewModel>().ToView("consumerMeters");
            modelBuilder.Entity<AddDeviceApiModel>().HasNoKey().ToView("addDeviceApis");
            modelBuilder.Entity<AssignMeterForSingle>().HasNoKey().ToView("assignMeterFors");
            modelBuilder.Entity<CheckNewMeterID>().HasNoKey().ToView("meterIDs");



            #endregion
        }
    }
}
