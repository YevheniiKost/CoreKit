using System;

using NUnit.Framework;

using YellowTape.CoreKit.Time;

namespace YeKostenko.CoreKit.Tests.EditMode.Time
{
    public sealed class LayerClockTests
    {
        private const float FrameDelta = 0.25f;

        private sealed class CountingListener : ITimeListener
        {
            public int UpdateCount { get; private set; }

            public float TotalDelta { get; private set; }

            public void Update(float deltaTime)
            {
                UpdateCount++;
                TotalDelta += deltaTime;
            }
        }

        [Test]
        public void Tick_WhenRunning_AdvancesTimeAndDelta()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);

            clock.Tick(FrameDelta);
            clock.Tick(FrameDelta);

            Assert.AreEqual(FrameDelta, clock.DeltaTime);
            Assert.AreEqual(FrameDelta * 2f, clock.Time);
        }

        [Test]
        public void Tick_WhenPaused_KeepsTimeAndZeroesDelta()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            clock.Tick(FrameDelta);
            object token = new object();

            clock.Pause(token);
            clock.Tick(FrameDelta);

            Assert.AreEqual(0f, clock.DeltaTime);
            Assert.AreEqual(FrameDelta, clock.Time);
        }

        [Test]
        public void Tick_WhenPaused_DoesNotUpdateListeners()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            CountingListener listener = new CountingListener();
            clock.RegisterTimeListener(listener);

            clock.Pause(this);
            clock.Tick(FrameDelta);

            Assert.AreEqual(0, listener.UpdateCount);
        }

        [Test]
        public void Resume_AfterPause_ContinuesFromSameTime()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            clock.Tick(FrameDelta);
            clock.Pause(this);
            clock.Tick(FrameDelta);

            clock.Resume(this);
            clock.Tick(FrameDelta);

            Assert.AreEqual(FrameDelta * 2f, clock.Time);
        }

        [Test]
        public void Resume_WithOneOfTwoTokens_StaysPaused()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            object menuToken = new object();
            object cutsceneToken = new object();
            clock.Pause(menuToken);
            clock.Pause(cutsceneToken);

            clock.Resume(menuToken);

            Assert.IsTrue(clock.IsPaused);
        }

        [Test]
        public void Resume_WithAllTokens_Unpauses()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            object menuToken = new object();
            object cutsceneToken = new object();
            clock.Pause(menuToken);
            clock.Pause(cutsceneToken);

            clock.Resume(menuToken);
            clock.Resume(cutsceneToken);

            Assert.IsFalse(clock.IsPaused);
        }

        [Test]
        public void Resume_WithUnknownToken_DoesNothing()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            clock.Pause(this);

            clock.Resume(new object());

            Assert.IsTrue(clock.IsPaused);
        }

        [Test]
        public void PausedChanged_FiresOnlyOnStateChange()
        {
            LayerClock clock = new LayerClock(TimeLayer.Game);
            int changeCount = 0;
            clock.PausedChanged += (object sender, EventArgs args) => changeCount++;
            object firstToken = new object();
            object secondToken = new object();

            clock.Pause(firstToken);
            clock.Pause(secondToken);
            clock.Pause(firstToken);
            clock.Resume(firstToken);
            clock.Resume(secondToken);
            clock.Resume(secondToken);

            Assert.AreEqual(2, changeCount);
        }
    }
}
