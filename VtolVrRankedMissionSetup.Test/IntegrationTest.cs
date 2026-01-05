using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.Services;
using VtolVrRankedMissionSetup.Services.ScenarioCreation;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;


namespace VtolVrRankedMissionSetup.Test
{
    [TestClass]
    public sealed class IntegrationTest
    {
        public TestContext TestContext { get; set; }

        private ScenarioModeService modeService = new();
        private AirbaseLayoutService airbaseLayoutService = new();
        private static VTMapCustom map = null!;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            // This is done statically to avoid IO conflicts when trying to access the file
            map = VTSerializer.DeserializeFromFile<VTMapCustom>("../../../../TestFiles/map.vtm");
        }

        [TestMethod]
        public async Task FiveFiveOneBaseline()
        {
            modeService.ActiveMode = modeService.Configs["551"];

            FiveFiveOneScenarioCreationService service = new(modeService, airbaseLayoutService);
            CustomScenario scenario = new(map);

            scenario.Bases[scenario.Bases.Length / 2].Layout = "551 Reversed";

            await ConfirmBaseline(service, scenario);
        }

        [TestMethod]
        public async Task HSBaseline()
        {
            modeService.ActiveMode = modeService.Configs["HS"];

            HSScenarioCreationService service = new(modeService, airbaseLayoutService);
            CustomScenario scenario = new(map);

            await ConfirmBaseline(service, scenario);
        }

        private async Task ConfirmBaseline(ScenarioCreationService service, CustomScenario scenario)
        {

            scenario.ScenarioName = TestContext.TestName;
            scenario.ScenarioID = TestContext.TestName;

            BaseInfo[] teamABases = scenario.Bases[..(scenario.Bases.Length / 2)];
            BaseInfo[] teamBBases = scenario.Bases[(scenario.Bases.Length / 2)..];

            foreach (BaseInfo baseInfo in teamBBases)
            {
                baseInfo.BaseTeam = Team.Enemy;
            }

            service.SetUpScenario(scenario, teamABases, teamBBases);

            VTSerializer.SerializeToFile(scenario, $"{TestContext.TestName}_Actual.vts");

            Task<string> expectedTask = File.ReadAllTextAsync($"../../../../TestFiles/{TestContext.TestName}_Expected.vts");
            Task<string> actualTask = File.ReadAllTextAsync($"{TestContext.TestName}_Actual.vts");

            TestContext.AddResultFile($"{TestContext.TestName}_Actual.vts");
            TestContext.WriteLine("Comparing files");

            if (await expectedTask != await actualTask)
            {
                Process process = new();
                Process devenv = Process.GetProcessesByName("devenv").First();
                process.StartInfo = new(devenv.MainModule!.FileName, $"/Diff ../../../../TestFiles/{TestContext.TestName}_Expected.vts {TestContext.TestName}_Actual.vts");
                process.Start();

                Assert.Fail("Files are not identical");
            }
        }
    }
}
