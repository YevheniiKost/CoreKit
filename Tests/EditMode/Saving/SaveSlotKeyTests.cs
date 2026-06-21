using System;

using NUnit.Framework;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    public sealed class SaveSlotKeyTests
    {
        [Test]
        public void ToStorageKey_ManualSlots_ReturnsExpectedNames()
        {
            Assert.AreEqual("slot_0", SaveSlotKey.ToStorageKey(SaveSlotId.Slot0));
            Assert.AreEqual("slot_4", SaveSlotKey.ToStorageKey(SaveSlotId.Slot4));
        }

        [Test]
        public void ToStorageKey_AutoSave_ReturnsAutosave()
        {
            Assert.AreEqual("autosave", SaveSlotKey.ToStorageKey(SaveSlotId.AutoSave));
        }

        [Test]
        public void FromManualSlotIndex_ReturnsMatchingSlotId()
        {
            Assert.AreEqual(SaveSlotId.Slot2, SaveSlotKey.FromManualSlotIndex(2));
        }

        [Test]
        public void FromManualSlotIndex_OutOfRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SaveSlotKey.FromManualSlotIndex(5));
            Assert.Throws<ArgumentOutOfRangeException>(() => SaveSlotKey.FromManualSlotIndex(-1));
        }

        [Test]
        public void IsManualSlot_DistinguishesAutoSave()
        {
            Assert.IsTrue(SaveSlotKey.IsManualSlot(SaveSlotId.Slot0));
            Assert.IsFalse(SaveSlotKey.IsManualSlot(SaveSlotId.AutoSave));
        }
    }
}
