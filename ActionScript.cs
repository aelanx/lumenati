using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumenati
{
    class ActionScript
    {
        public enum Opcode : byte
        {
            End = 0x00,
            NextFrame = 0x04,
            PreviousFrame = 0x05,
            Play = 0x06,
            Stop = 0x07,
            ToggleQuality = 0x08,
            StopSounds = 0x09,
            Add = 0x0A,
            Subtract = 0x0B,
            Multiply = 0x0C,
            Divide = 0x0D,
            Equals = 0x0E,
            Less = 0x0F,
            LogicalAnd = 0x10,
            LogicalOr = 0x11,
            Not = 0x12,
            StringEquals = 0x13,
            StringLength = 0x14,
            StringExtract = 0x15,
            Pop = 0x17,
            ToInteger = 0x18,
            GetVariable = 0x1C,
            SetVariable = 0x1D,
            SetTarget2 = 0x20,
            StringAdd = 0x21,
            GetProperty = 0x22,
            SetProperty = 0x23,
            CloneSprite = 0x24,
            RemoveSprite = 0x25,
            Trace = 0x26,
            StartDrag = 0x27,
            EndDrag = 0x28,
            StringLess = 0x29,
            Throw = 0x2A,
            CastOp = 0x2B,
            Implements = 0x2C,
            RandomNumber = 0x30,
            MBStringLength = 0x31,
            CharToAscii = 0x32,
            AsciiToChar = 0x33,
            GetTime = 0x34,
            MBStringExtract = 0x35,
            MBCharToAscii = 0x36,
            MBAsciiToChar = 0x37,
            Delete = 0x3A,
            Delete2 = 0x3B,
            DefineLocal = 0x3C,
            CallFunction = 0x3D,
            Return = 0x3E,
            Modulo = 0x3F,
            NewObject = 0x40,
            DefineLocal2 = 0x41,
            InitArray = 0x42,
            InitObject = 0x43,
            TypeOf = 0x44,
            TargetPath = 0x45,
            Enumerate = 0x46,
            Add2 = 0x47,
            Less2 = 0x48,
            Equals2 = 0x49,
            ToNumber = 0x4A,
            ToString = 0x4B,
            PushDuplicate = 0x4C,
            StackSwap = 0x4D,
            GetMember = 0x4E,
            SetMember = 0x4F,
            Increment = 0x50,
            Decrement = 0x51,
            CallMethod = 0x52,
            NewMethod = 0x53,
            InstanceOf = 0x54,
            Enumerate2 = 0x55,
            BitwiseAnd = 0x60,
            BitwiseOr = 0x61,
            BitwiseXor = 0x62,
            BitwiseLeftShift = 0x63,
            SignedBitwiseRightShift = 0x64,
            UnsignedBitwiseRightShift = 0x65,
            StrictEquals = 0x66,
            TypedGreaterThan = 0x67,
            StringGreaterThan = 0x68,
            Extends = 0x69,

            GotoFrame = 0x81,
            GetURL = 0x83,
            StoreRegister = 0x87,
            ConstantPool = 0x88,
            WaitForFrame = 0x8A,
            SetTarget = 0x8B,
            GoToLabel = 0x8C,
            WaitForFrame2 = 0x8D,
            DefineFunction2 = 0x8E,
            With = 0x94,
            Push = 0x96,
            Jump = 0x99,
            GetURL2 = 0x9A,
            DefineFunction = 0x9B,
            If = 0x9D,
            Call = 0x9E,
            GotoFrame2 = 0x9F
        }

        public enum Type
        {
            String = 0,
            Float = 1,
            Null = 2,
            Undefined = 3,
            Register = 4,
            Boolean = 5,
            Double = 6,
            Integer = 7,
            Constant8 = 8,
            Constant16 = 9
        }

        public ActionScript(InputBuffer file)
        {
            var numActions = file.readInt();

            for (int actionId = 0; actionId < numActions; actionId++)
            {
                var actionBytes = file.readInt();
                int bytesRead = 0;

                while (bytesRead < actionBytes)
                {
                    var opcode = (Opcode)file.readByte();
                    short instructionLength = 1;

                    if ((byte)opcode >= 0x80)
                        instructionLength = file.readShortLE();

                    switch (opcode)
                    {
                        default:
                        file.ptr += (uint)instructionLength;
                        break;
                    }
                    /////
                    /////
                }

                var padLength = (4 - (bytesRead % 4)) % 4;
                file.ptr += (uint)padLength;
            }
        }
    }
}
