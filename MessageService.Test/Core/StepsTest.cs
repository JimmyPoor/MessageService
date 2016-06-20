using MessageService.Core;
using MessageService.Core.Container.Unity;
using MessageService.Core.Steps;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Test.Core
{
    [TestFixture]
    public class SimpleStepsTest
    {
        List<TestStep> _testSteps;
        List<ErrorStep> _errorSteps;
        List<LongRunTestStep> _longRunningSteps;
        //StepContextMapper _contextMapper;
        int _defaultStepsCount = 5;
        int _errorStepsCount = 2;
        int _longRunStepsCount = 3;
        int _longRunSecond = 3;

        [SetUp]
        public void Setup()
        {
            BuildTestStep();
            BuildErrorStep();
            BuildLongRunStep();
        }

        [Test]
        public void Create_ServiceSteps_Instanc_Test()
        {
            var steps = ServiceSteps.Create();
            Assert.IsNotNull(steps);
        }

        [Test]
        public void Registe_Step_Test()
        {
            var _steps = new ServiceSteps();
            var step = new TestStep("step1", 1);
            _steps.RegistStep(step);
            var count2 = _steps.Count;
            Assert.AreEqual(count2, 1);

            var steps = _testSteps;
            _steps.RegistSteps(steps);
            var count3 = _steps.Count;

            Assert.AreEqual(count3, count2 += _testSteps.Count);
        }

        [Test]
        public void Inovke_Steps_without_context_Test()
        {
            Assert.Catch<NullReferenceException>(() =>
            {
                var _steps = new ServiceSteps();
                _steps.RegistSteps(_testSteps);
                var ctx = _steps.StartSteps(null); // will throw null exception if step context is null
                Assert.IsNull(ctx);
                Assert.AreEqual(TestStep.InvokeCount, _defaultStepsCount);
            },"step context can't be null when invoke steps");
        }

        [Test]
        public void Inovke_Steps_with_context_not_throw_in_each_step_Test()
        {
            TestStep.InvokeCount = 0;
            var stepContext = CreateContext();
            var _steps = new ServiceSteps();
            _steps.RegistSteps(_testSteps);
            var ctx = _steps.StartSteps(stepContext);
            Assert.IsNotNull(ctx);
            Assert.AreEqual(TestStep.InvokeCount, _defaultStepsCount);
        }

        [Test]
        public async void Invoke_Steps_with_context_throw_error_in_step_test()
        {
            var stepContext = CreateContext();
            var _steps = new ServiceSteps();
            _steps.RegistSteps(_errorSteps);
            var ctx = await _steps.StartSteps(stepContext);

            Assert.AreEqual(ctx.Errors.Count, _errorStepsCount);
            Assert.IsTrue(ctx.Errors.All(error => error.InnerException.GetType() == typeof(NotImplementedException)));
        }

        [Test]
        public async  void Invoke_longRun_steps()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var stepContext = CreateContext();
            var _steps = new ServiceSteps();
            _steps.RegistSteps(_longRunningSteps);
            var ctx = await _steps.StartSteps(stepContext);
            watch.Stop();
            Assert.GreaterOrEqual(watch.Elapsed.Seconds, _longRunSecond * _longRunStepsCount);
        }


        [Test]
        public void Find_Steps_without_any_assembly_Test()
        {
            //var steps = StepsManager.FindStepsFromAssemblies<IStep>();
            //Assert.IsNotNull(steps);
            //Assert.AreEqual(steps.Count(), 0);
        }

        [Test]
        public void Find_Steps_with_any_assembly_Test()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var steps = StepsManager.FindStepsFromAssemblies<IStep>(assemblies);
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Count() > 0);
        }

        [Test]
        public void Step_Context_Mapping_Test()
        {

        }

        private void BuildTestStep()
        {
            _testSteps = new List<TestStep>();
            for (int i = 0; i < _defaultStepsCount; i++)
            {
                _testSteps.Add(
                    new TestStep("step" + i, i)
                    );
            }
        }
        private void BuildErrorStep()
        {
            _errorSteps = new List<ErrorStep>();
            for (int i = 0; i < 2; i++)
            {
                _errorSteps.Add(new ErrorStep());
            }

        }

        private void BuildLongRunStep()
        {
            _longRunningSteps = new List<LongRunTestStep>();
            for (int i = 0; i < _longRunStepsCount; i++)
            {
                _longRunningSteps.Add(new LongRunTestStep() {  RunSecond=TimeSpan.FromSeconds(_longRunSecond)});
            }
        }

        private StepProcessContext CreateContext()
        {
            return new StepProcessContext(new ProcessContext(
                 new UnityContainer(),
                 new Settings()
                ));
        }
    }


    public class TestStep : IOutgoingStep
    {
        public static int InvokeCount { get; set; } = 0;
        public bool IsEnable { get; set; } = true;

        public string StepName { get; private set; }

        public int StepNo { get; private set; }

        public Guid StepId => Guid.NewGuid();

        public TestStep()
        {

        }

        public TestStep(string stepName, int stepNo)
        {
            this.StepName = stepName;
            this.StepNo = stepNo;
        }

        public virtual Task ExcuteStep(StepProcessContext ctx)
        {
            InvokeCount++;
            Console.WriteLine(this.ToString());
            return Task.FromResult(0);
        }

        public override string ToString()
        {
            return string.Format("StepName: {0} and StepNo: {1}", StepName, StepNo);
        }

    }

    public class ErrorStep : TestStep
    {
        public override Task ExcuteStep(StepProcessContext ctx)
        {
            throw new NotImplementedException();
        }
    }

    public class LongRunTestStep : IStep
    {
      
        public bool IsEnable { get; set; }

        public Guid StepId => Guid.NewGuid();

        public string StepName { get; }

        public int StepNo { get; }

        public TimeSpan RunSecond { get; set; }

        public async Task ExcuteStep(StepProcessContext ctx)
        {
            await Task.Run(async () =>
            {
                    await Task.Delay(RunSecond);
            });
        }
    }

}
