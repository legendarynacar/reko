﻿#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class ArmRewriterTests : RewriterTestBase
    {
        private Arm32Architecture arch = new Arm32Architecture("arm32");
        private MemoryArea image;
        private Address baseAddress = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddress; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(new LeImageReader(image, 0), new ArmProcessorState(arch), binder, host);
        }

        private void BuildTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            image = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }

        private void BuildTest(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            image = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }

        [Test]
        public void ArmRw_mov_r1_r2()
        {
            BuildTest("1110 00 0 1101 0 0000 0001 00000000 0010");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2");
        }

        [Test]
        public void ArmRw_add_r1_r2_r3()
        {
            BuildTest("1110 00 0 0100 0 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 + r3");
        }

        [Test]
        public void ArmRw_adds_r1_r2_r3()
        {
            BuildTest("1110 00 0 0100 1 0010 0001 00000000 0011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r2 + r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_subgt_r1_r2_imm4()
        {
            BuildTest("1100 00 1 0010 0 0010 0001 0000 00000100");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,NZV)) branch 00100004",
                "2|L--|r1 = r2 - 0x00000004");
        }

        [Test]
        public void ArmRw_orr_r3_r4_r5_lsl_5()
        {
            BuildTest("1110 00 0 1100 0 1100 0001 00100 000 0100");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = ip | r4 << 4");
        }

        [Test]
        public void ArmRw_brgt()
        {
            BuildTest("1100 1010 000000000000000000000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GT,NZV)) branch 00100008");
        }

        [Test]
        public void ArmRw_lsl()
        {
            BuildTest(0xE1A00200);  // mov\tr0,r0,lsl #4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 << 4");
        }

        [Test]
        public void ArmRw_stmdb()
        {
            BuildTest(0xE92C003B);  // stmdb ip!,{r0,r1,r3-r5},lr,pc}
            AssertCode(
                "0|L--|00100000(4): 6 instructions",
                "1|L--|Mem0[ip - 4:word32] = r0",
                "2|L--|Mem0[ip - 8:word32] = r1",
                "3|L--|Mem0[ip - 12:word32] = r3",
                "4|L--|Mem0[ip - 16:word32] = r4",
                "5|L--|Mem0[ip - 20:word32] = r5",
                "6|L--|ip = ip - 24");
        }

        [Test]
        public void ArmRw_bllt()
        {
            BuildTest(0xBB000330);  // bllt
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,NZV)) branch 00100004",
                "2|T--|call 00100CC8 (0)");
        }

        [Test]
        public void ArmRw_ldr()
        {
            BuildTest(0xE5940008);  // ldr r0,[r4,#8]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r4 + 8:word32]");
        }

        [Test]
        public void ArmRw_bne()
        {
            BuildTest(0x1A000004);  // bne
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100018");
        }

        [Test]
        public void ArmRw_bic()
        {
            BuildTest(0xE3CEB3FF);  // bic
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = lr & ~0xFC000003");
        }

        [Test]
        public void ArmRw_mov_pc_lr()
        {
            BuildTest(0xE1B0F00E);  // mov pc,lr
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void ArmRw_ldrsb()
        {
            BuildTest(0xE1F120D1);  // ldrsb r2,[r1,#1]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + 1",
                "2|L--|r2 = (word32) Mem0[r1:int8]");
        }

        [Test]
        public void ArmRw_mov_pc()
        {
            BuildTest(0xE59F0010);  // ldr\tr0,[pc,#&10]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[0x00100018:word32]");
        }

        [Test]
        public void ArmRw_cmp()
        {
            BuildTest(0xE3530000);  // cmp r3,#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 - 0x00000000)");
        }

        [Test]
        public void ArmRw_cmn()
        {
            BuildTest(0xE3730001); /// cmn r3,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(r3 + 0x00000001)");
        }

        [Test]
        public void ArmRw_ldr_pc()
        {
            BuildTest(0xE59CF000); // ldr pc,[ip]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[ip:word32]");
        }

        [Test]
        public void ArmRw_ldr_post()
        {
            BuildTest(0xE4D43001);// ldrb r3,[r4],#1
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = (word32) Mem0[r4:byte]",
                "2|L--|r4 = r4 + 0x00000001",
                "3|L--|r3 = v4");
        }

        [Test]
        public void ArmRw_push()
        {
            BuildTest(0xE92D4010);
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|sp = sp - 8",
               "2|L--|Mem0[sp:word32] = r4",
               "3|L--|Mem0[sp + 4:word32] = lr");
        }

        [Test]
        public void ArmRw_movw()
        {
            BuildTest(0xE30F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = 0x0000FFFF");
        }

        [Test]
        public void ArmRw_uxtb()
        {
            BuildTest(0xE6EF2071);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (uint32) (byte) r1");
            BuildTest(0xE6EF2471);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (uint32) (byte) (r1 >>u 8)");
        }

        [Test]
        public void ArmRw_bxuge()
        {
            BuildTest(0x212FFF1E);
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULT,C)) branch 00100004",
                "2|T--|goto lr");
        }

        [Test]
        public void ArmRw_movt()
        {
            BuildTest(0xE34F4FFF);
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r4 = DPB(r4, 0xFFFF, 16)");
        }

        [Test]
        public void ArmRw_pop()
        {
            BuildTest(0xE8BD000C);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r2 = Mem0[sp:word32]",
                "2|L--|r3 = Mem0[sp + 4:word32]",
                "3|L--|sp = sp + 8");
        }

        [Test]
        public void ArmRw_popne()
        {
            BuildTest(0x18BD000C);
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r2 = Mem0[sp:word32]",
                "3|L--|r3 = Mem0[sp + 4:word32]",
                "4|L--|sp = sp + 8");
        }

        [Test]
        public void ArmRw_clz()
        {
            BuildTest(0xE16F4F13);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __clz(r3)");
        }

        [Test]
        public void ArmRw_strd()
        {
            BuildTest(0xE04343F8);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r3:word64] = r5_r4",
                "2|L--|r3 = r3 + 0xFFFFFFC8");
        }

        [Test]
        public void ArmRw_muls()
        {
            BuildTest(0xE0120A94);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r4 * r10",
                "2|L--|NZCV = cond(r2)");
        }

        [Test]
        public void ArmRw_mlas()
        {
            BuildTest(0xE0314392);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r4 + r2 * r3",
                "2|L--|NZCV = cond(r1)");
        }

        [Test]
        public void ArmRw_bfi()
        {
            BuildTest(0xE7CD1292);
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|v4 = SLICE(r2, ui9, 0)",
               "2|L--|r1 = DPB(r1, v4, 5)");
        }

        /*
        MOV             R2, R0,LSR#8
        MOV             R3, #0
        AND             R0, R0, #0xF
        BFI             R3, R2, #0, #8
        SUB             SP, SP, #8
        BFI             R3, R0, #8, #8
        MOV             R0, R3
        ADD             SP, SP, #8
        BX              LR
        int __fastcall AUDIO_ConvertVolumeValue(unsigned __int16 a1)
        {
          return (a1 >> 8) | ((a1 & 0xF) << 8);
        }

        MOV             R3, #0
MOV             R1, #0xD
MOV             R0, R3
BFI             R0, R1, #0, #8
BFI             R0, R3, #8, #8
BFI             R0, R2, #0x10, #8
ADD             SP, SP, #8
BX              LR
means
  return (v3 << 16) | 0xD;
         */

        [Test]
        public void ArmRw_ldrd()
        {
            BuildTest(0xE1C722D8);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3_r2 = Mem0[r7 + 40:word64]");
        }

        [Test]
        public void ArmRw_ubfx()
        {
            BuildTest(0xE7F01252);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (uint32) SLICE(r2, ui17, 4)");
        }

        [Test]
        public void ArmRw_sxtb()
        {
            BuildTest(0xE6AF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (int32) (int8) (r2 >>u 8)");
        }

        [Test]
        public void ArmRw_uxth()
        {
            BuildTest(0xE6FF1472);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (uint32) (uint16) (r2 >>u 8)");
        }

        [Test]
        public void ArmRw_umull()
        {
            BuildTest(0xE0912394);
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1_r2 = r3 *u r4",
                "2|L--|NZCV = cond(r1_r2)");
        }

        [Test]
        public void ArmRw_mls()
        {
            BuildTest(0xE0612394);
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 - r4 * r3");
        }

        [Test]
        public void ArmRw_vldmia()
        {
            //04 0B F2 EC
            //04 0B E3 EC
            BuildTest(0xECF20B04);
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|d16 = Mem0[r2:word64]",
                "2|L--|d17 = Mem0[r2 + 8:word64]",
                "3|L--|r2 = r2 + 16");
        }

        [Test]
        public void ArmRw_ldmib()
        {
            BuildTest(0xE9950480); // ldmibr5, r7, r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r7 = Mem0[r5 + 4:word32]",
                "2|L--|r10 = Mem0[r5 + 8:word32]");
        }

        [Test]
        public void ArmRw_rev()
        {
            BuildTest(0xE6BF2F32); // rev r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __rev(r2)");
        }

        [Test]
        public void ArmRw_vmov_i32()
        {
            //  51 00 C0 F2 
            BuildTest(0xF2C00051); // vmov.i32 q8,#1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = SEQ(0x00000001, 0x00000001, 0x00000001, 0x00000001)");
        }

        [Test]
        public void ArmRw_adc()
        {
            BuildTest(0xE0A22002); // adc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|r2 = r2 + r2 + C");
        }

        [Test]
        public void ArmRw_sbc()
        {
            BuildTest(0xE0C22002); // sbc r2,r2,r2
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|r2 = r2 - r2 - C");
        }

        [Test]
        public void ArmRw_vstmia()
        {
            BuildTest(0xECE30B04);  // vstmia r3, d16, d17
            AssertCode(
               "0|L--|00100000(4): 3 instructions",
               "1|L--|Mem0[r3:word64] = d16",
               "2|L--|Mem0[r3 + 8:word64] = d17",
               "3|L--|r3 = r3 + 16");
        }

        [Test]
        public void ArmRw_mrs()
        {
            BuildTest(0xE10F3000); // mrs r3, cpsr
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|r3 = __mrs(apsr)");
        }

        [Test]
        public void ArmRw_cpsid()
        {
            BuildTest(0xF10C0080); // cpsid
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__cps_id()");
        }

        [Test]
        public void ArmRw_smulbb()
        {
            BuildTest(0xE1600380); //  smulbb r0, r0, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (int16) r0 *s (int16) r3");
        }

        [Test]
        public void ArmRw_bfc()
        {
            BuildTest(0xE7C5901F);  // bfc r9, #0, #6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r9 & 0xFFFFFFC0");
        }

        [Test]
        public void ArmRw_sbfx()
        {
            BuildTest(0xE7A9C35C); // sbfx ip,ip,#6,#&A
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = (int32) SLICE(ip, ui10, 6)");
        }

        [Test]
        public void ArmRw_umlalne()
        {
            BuildTest(0x10A54A93); // umlalne r4,r5,r3,r10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|r5_r4 = r3 *u r10 + r5_r4");
        }

        [Test]
        public void ArmRw_msr()
        {
            BuildTest(0xE121F001); // msr cpsr, r1
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|__msr(cpsr, r1)");
        }

        [Test]
        public void ArmRw_uxtab()
        {
            BuildTest(0xE6E10070);  // uxtab r0, r1, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r1 + (byte) r0");
        }

        [Test]
        public void ArmRw_sxtab()
        {
            BuildTest(0xE6A55078);  // sxtab r5, r5, r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 + (int8) r8");
        }

        [Test]
        public void ArmRw_sxtah()
        {
            BuildTest(0xE6B6A07A);  // sxtah r10,r6,r10
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|r10 = r6 + (int16) r10");
        }

        [Test]
        public void ArmRw_sxthne()
        {
            BuildTest(0x16BF9077);  //  sxthne r9,r7
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004");
        }

        [Test]
        public void ArmRw_uxtah()
        {
            BuildTest(0xE6F30072);  // uxtah r0,r3,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r3 + (uint16) r2");
        }

        [Test]
        public void ArmRw_dmb()
        {
            BuildTest(0xF57FF05F);  // dmb
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|__dmb_sy()");
        }

        [Test]
        public void ArmRw_mrc()
        {
            BuildTest(0xEE123F10);  // mrc p15,#0,r3,c2
            AssertCode(
             "0|L--|00100000(4): 1 instructions",
             "1|L--|r3 = __mrc(p15, 0x00000000, 0x02, 0x00, 0x00000000)");
        }

        [Test]
        public void ArmRw_mcr()
        {
            BuildTest(0xEE070F58);  // mcr p15,#0,r0,c7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mcr(p15, 0x00000000, r0, 0x07, 0x08, 0x00000002)");
        }

        [Test]
        public void ArmRw_bl()
        {
            BuildTest(0xEB00166B);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 001059B4 (0)");
        }

        [Test]
        [Ignore(Categories.Capstone)]
        public void ArmRw_rsc()
        {
            // Capstone incorrectly disassembles this as setting the S flag.
            BuildTest(0x00E050CC); // rsceq asr r5,r0,ip, asr #1
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r5 = (ip >> 1) - r0 - C",
                "3|L--|r5 = ip - r0");
        }

        [Test]
        public void ArmRw_smlawt()
        {
            BuildTest(0x012D06CC); // smlawteq sp,ip,r6,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|sp = (ip *s (int16) (r6 >> 16) >> 16) + r0");
            //74 EC 0A 01 ????
            //﻿90 41 E0 00 smlaleqr4,r0,r0,r1
            // B0 44 E0 00 strhteqr4,[r0],#&40
            //﻿ A8 5B 2E 01 smulwbeqlr,r8,fp
        }

        [Test]
        public void ArmRw_smlal()
        {
            BuildTest(0xE0e04190);	// smlal r4, r0, r0, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_r0 = r0 *s r1 + r4_r0");
        }

        [Test]
        public void ArmRw_strht()
        {
            BuildTest(0xE0e051b0);	// strht r5, [r0], #0x10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r0:word16] = (uint16) r5");
        }

        [Test]
        public void ArmRw_swpeq()
        {
            BuildTest(0xE10ea598);	// swp sl, r8, [lr]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = std::atomic_exchange<int32_t>(r8, Mem0[lr:word32])");
        }

        [Test]
        public void ArmRw_smulwb()
        {
            BuildTest(0xE12e5ba8);	// smulwb lr, r8, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = r8 *s (int16) fp >> 16");
        }

        [Test]
        public void ArmRw_smulbt()
        {
            BuildTest(0xE168dbcc);	// smulbt r8, ip, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = (int16) ip *s (int16) (fp >> 16)");
        }

        [Test]
        public void ArmRw_qdsub()
        {
            BuildTest(0xE168da50);	// qdsub sp, r0, r8
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|sp = __signed_sat_32(r8 - __signed_sat_32(r0 *s 2))",
                "2|L--|Q = cond(sp)");
        }

        [Test]
        public void ArmRw_ldrsht()
        {
            BuildTest(0xE0fe50fc);	// ldrsht r5, [lr], #0xc
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = (word32) Mem0[lr:int16]",
                "2|L--|lr = lr + 0x0000000C",
                "3|L--|r5 = v4");
        }

        [Test]
        public void ArmRw_smultt()
        {
            BuildTest(0xE168dbe0);	// smultt r8, r0, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = (int16) (r0 >> 16) *s (int16) (fp >> 16)");
        }

        [Test]
        public void ArmRw_qadd()
        {
            BuildTest(0xE10fb85c);	// qadd fp, ip, pc
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|fp = __signed_sat_32(ip + 0x00100008)",
                "2|L--|Q = cond(fp)");
        }

        [Test]
        public void ArmRw_qsub()
        {
            BuildTest(0xE12d6650);	// qsube r6, r0, sp
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = __signed_sat_32(r0 - sp)",
                "2|L--|Q = cond(r6)");
        }

        [Test]
        public void ArmRw_smlatb()
        {
            BuildTest(0xE10c6ca0);	// smlatb ip, r0, ip, r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|ip = (int16) (r0 >> 16) *s (int16) ip + r6",
                "2|L--|Q = cond(ip)");
        }

        [Test]
        public void ArmRw_ldrht()
        {
            BuildTest(0xE0fd52b4);	// ldrht r5, [sp], #0x24
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = (word32) Mem0[sp:word16]",
                "2|L--|sp = sp + 0x00000024",
                "3|L--|r5 = v4");
        }

        [Test]
        public void ArmRw_smulwt()
        {
            BuildTest(0xE1206aec);	// smulwt r0, ip, sl
            AssertCode("0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = ip *s (int16) (r10 >> 16) >> 16");
        }

        [Test]
        public void ArmRw_smlawb()
        {
            BuildTest(0xE12d5980);	// smlawb sp, r0, sb, r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = (r0 *s (int16) r9 >> 16) + r5");
        }

        [Test]
        public void ArmRw_ldrsbteq()
        {
            BuildTest(0x00f707d0);	// ldrsbteq r0, [r7], #0x70
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|v5 = (word32) Mem0[r7:int8]",
                "3|L--|r7 = r7 + 0x00000070",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_smultb()
        {
            BuildTest(0xE16c69ac);	// smultb ip, ip, sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = (int16) (ip >> 16) *s (int16) r9");
        }

        [Test]
        public void ArmRw_vstr()
        {
            BuildTest(0xedcd0b29);	// vstr d16, [sp, #0xa4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 164:word64] = d16");
        }

        [Test]
        public void ArmRw_vldr()
        {
            BuildTest(0xedd20b04);	// vldr d16, [r2, #0x10]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = Mem0[r2 + 16:word64]");
        }

        [Test]
        public void ArmRw_veor()
        {
            BuildTest(0xf34001f4);	// veor q8, q8, q10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q8 ^ q10");
        }

        [Test]
        public void ArmRw_vext_64()
        {
            BuildTest(0xf2f068e2);	// vext.64 q11, q8, q9, #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q11 = __vext(q8, q9, 0x00000001)");
        }

        [Test]
        public void ArmRw_vmov_32()
        {
            BuildTest(0xee102b90);	// vmov.32 r2, d16[0]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = d16");
        }

        [Test]
        public void ArmRw_smlabt()
        {
            BuildTest(0xE10f54cc);  // smlabt pc, ip, r4, r5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|pc = (int16) ip *s (int16) (r4 >> 16) + r5",
                "2|L--|Q = cond(pc)");
        }

        [Test]
        public void ArmRw_vcvt_f64_s32()
        {
            BuildTest(0xeef80be7);  // vcvt.f64.s32 d16, s15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = (real64) s15");
        }

        [Test]
        public void ArmRw_vpush()
        {
            BuildTest(0xed2d8b04);  // vpush {d8, d9}
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|sp = sp - 16",
                "2|L--|Mem0[sp:word64] = d8",
                "3|L--|Mem0[sp + 8:word64] = d9");
        }

        [Test]
        public void ArmRw_vpop()
        {
            BuildTest(0xecbd8b04);  // vpop {d8, d9}
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|d8 = Mem0[sp:word64]",
                "2|L--|d9 = Mem0[sp + 8:word64]",
                "3|L--|sp = sp + 16");
        }

        [Test]
        public void ArmRw_vsub_f64()
        {
            BuildTest(0xee711be0);  // vsub.f64 d17, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d17 = __vsub_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_vmul_f64()
        {
            BuildTest(0xee611ba0);  // vmul.f64 d17, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d17 = __vmul_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_vdiv_f64()
        {
            BuildTest(0xee817ba0);  // vdiv.f64 d7, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = d17 / d16");
        }

        [Test]
        public void ArmRw_vcmpe_f32()
        {
            BuildTest(0xeeb49ae7);  // vcmpe.f32 s18, s15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s18 - s15)");
        }

        [Test]
        public void ArmRw_vmrs()
        {
            BuildTest(0xeef1fa10);  // vmrs apsr_nzcv, fpscr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|apsr_nzcv = fpscr");
        }

        [Test]
        public void ArmRw_vnmls_f32()
        {
            BuildTest(0xee567a87);  // vnmls.f32 s15, s13, s14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s15 = __vnmls_f32(s13, s14)");
        }

        [Test]
        public void ArmRw_vmla_f32()
        {
            BuildTest(0xee476a86);  // vmla.f32 s13, s15, s12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s13 = __vmla_f32(s15, s12)");
        }

        [Test]
        public void ArmRw_ldrbtgt()
        {
            BuildTest(0xc47a0000);  // ldrbtgt r0, [sl], #-0
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(LE,NZV)) branch 00100004",
                "2|L--|v5 = (word32) Mem0[r10:byte]",
                "3|L--|r10 = r10 + 0x00000000",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_vmax_s32()
        {
            BuildTest(0xf26006e2);  // vmax.s32 q8, q8, q9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = __vmax_s32(q8, q9)");
        }

        [Test]
        public void ArmRw_vpmax_s32()
        {
            BuildTest(0xf2600aa0);  // vpmax.s32 d16, d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vpmax_s32(d16, d16)");
        }

        [Test]
        public void ArmRw_vorr()
        {
            BuildTest(0xf26021b0);  // vorr d18, d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d18 = d16 | d16");
        }

        [Test]
        public void ArmRw_vmin_s32()
        {
            BuildTest(0xf26446f0);  // vmin.s32 q10, q10, q8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmin_s32(q10, q8)");
        }

        [Test]
        public void ArmRw_vpmin_s32()
        {
            BuildTest(0xf2644ab4);  // vpmin.s32 d20, d20, d20
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vpmin_s32(d20, d20)");
        }

        [Test]
        public void ArmRw_smlabb()
        {
            BuildTest(0xE10e3b88);  // smlabb lr, r8, fp, r3
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|lr = (int16) r8 *s (int16) fp + r3");
        }

        [Test]
        public void ArmRw_stmda()
        {
            BuildTest(0xE84230fd);  // stmda r2, {r0, r2, r3, r4, r5, r6, r7, ip, sp} ^
            AssertCode(
                "0|L--|00100000(4): 9 instructions",
                "1|L--|Mem0[r2:word32] = r0",
                "2|L--|Mem0[r2 - 4:word32] = r2",
                "3|L--|Mem0[r2 - 8:word32] = r3",
                "4|L--|Mem0[r2 - 12:word32] = r4",
                "5|L--|Mem0[r2 - 16:word32] = r5",
                "6|L--|Mem0[r2 - 20:word32] = r6",
                "7|L--|Mem0[r2 - 24:word32] = r7",
                "8|L--|Mem0[r2 - 28:word32] = ip",
                "9|L--|Mem0[r2 - 32:word32] = sp");
        }

        [Test]
        public void ArmRw_stmda_w()
        {
            BuildTest(0xE86230fd);  // stmda r2, {r0, r2, r3, r4, r5, r6, r7, ip, sp} ^
            AssertCode(
                "0|L--|00100000(4): 10 instructions",
                "1|L--|Mem0[r2:word32] = r0",
                "2|L--|Mem0[r2 - 4:word32] = r2",
                "3|L--|Mem0[r2 - 8:word32] = r3",
                "4|L--|Mem0[r2 - 12:word32] = r4",
                "5|L--|Mem0[r2 - 16:word32] = r5",
                "6|L--|Mem0[r2 - 20:word32] = r6",
                "7|L--|Mem0[r2 - 24:word32] = r7",
                "8|L--|Mem0[r2 - 28:word32] = ip",
                "9|L--|Mem0[r2 - 32:word32] = sp",
                "10|L--|r2 = r2 - 36");
        }

        [Test]
        public void ArmRw_vneg_f64()
        {
            BuildTest(0xeef10b60);  // vneg.f64 d16, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vneg_f64(d16)");
        }

        [Test]
        public void ArmRw_vnmul_f64()
        {
            BuildTest(0xee680b60);  // vnmul.f64 d16, d8, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vnmul_f64(d8, d16)");
        }

        [Test]
        public void ArmRw_cdplo()
        {
            BuildTest(0x3e200000);  // cdplo p0, #2, c0, c0, c0, #0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|L--|__cdp(p0, 0x00000002, 0x00, 0x00, 0x00, 0x00000000)");
        }

        [Test]
        public void ArmRw_vpadd_i32()
        {
            BuildTest(0xf2622bb2);  // vpadd.i32 d18, d18, d18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d18 = __vpadd_i32(d18, d18)");
        }

        [Test]
        public void ArmRw_strbt()
        {
            BuildTest(0xE6666666);  // strbt r6, [r6], -r6, ror #12
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r6:byte] = (byte) r6",
                "2|L--|r6 = r6 - __ror(r6, 12)");
        }

        [Test]
        public void ArmRw_stc()
        {
            BuildTest(0xECCCCCCD);  // stc p12, c12, [ip], {0xcd}
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stc(p12, 0x0C, Mem0[ip:word32])");
        }

        [Test]
        public void ArmRw_vdup_32()
        {
            BuildTest(0xeea02b90);	// vdup.32 q8, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = __vdup_32(r2)");
        }

        [Test]
        public void ArmRw_vmvn_i32()
        {
            BuildTest(0xf2c04077);  // vmvn.i32 q10, #7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmvn_imm_i32(0x00000007)");
        }

        [Test]
        public void ArmRw_vshl_u32()
        {
            BuildTest(0xf36424e2);  // vshl.u32 q9, q9, q10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = __vshl_u32(q9, q10)");
        }

        [Test]
        public void ArmRw_vmls_f64()
        {
            BuildTest(0xee017be0);  // vmls.f64 d7, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = __vmls_f64(d17, d16)");
        }

        [Test]
        public void ArmRw_ldmdalt()
        {
            BuildTest(0xb851eb85);  // ldmdalt r1, {r0, r2, r7, r8, sb, fp, sp, lr, pc} ^
            AssertCode(
                "0|T--|00100000(4): 10 instructions",
                "1|T--|if (Test(GE,NZV)) branch 00100004",
                "2|L--|r0 = Mem0[r1:word32]",
                "3|L--|r2 = Mem0[r1 - 4:word32]",
                "4|L--|r7 = Mem0[r1 - 8:word32]",
                "5|L--|r8 = Mem0[r1 - 12:word32]",
                "6|L--|r9 = Mem0[r1 - 16:word32]",
                "7|L--|fp = Mem0[r1 - 20:word32]",
                "8|L--|sp = Mem0[r1 - 24:word32]",
                "9|L--|lr = Mem0[r1 - 28:word32]",
                "10|T--|return (0,0)");
        }

        [Test]
        public void ArmRw_vabs_f64()
        {
            BuildTest(0xeeb09bc9);  // vabs.f64 d9, d9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d9 = __vabs_f64(d9)");
        }

        [Test]
        public void ArmRw_vadd_f64()
        {
            BuildTest(0xee377b20);  // vadd.f64 d7, d7, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d7 = __vadd_f64(d7, d16)");
        }

        [Test]
        public void ArmRw_vand()
        {
            BuildTest(0xf24001f2);  // vand q8, q8, q9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q8 & q9");
        }

        [Test]
        public void ArmRw_vcmp_f32()
        {
            BuildTest(0xeef47a47);  // vcmp.f32 s15, s14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s15 - s14)");
        }

        [Test]
        public void ArmRw_vsqrt_f64()
        {
            BuildTest(0xeeb1cbe0);  // vsqrt.f64 d12, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d12 = sqrt(d16)");
        }

        [Test]
        public void ArmRw_umaal()
        {
            BuildTest(0xE040a590);  // umaal sl, r0, r0, r5
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = r0 *u r5",
                "2|L--|v2 = v2 + (uint64) r0",
                "3|L--|r0_r10 = v2 + (uint64) r10");
        }

        [Test]
        public void ArmRw_smlatteq()
        {
            BuildTest(0x010bdae4);  // smlatteq fp, r4, sl, sp
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|fp = (int16) (r4 >> 16) *s (int16) (r10 >> 16) + sp",
                "3|L--|Q = cond(fp)");
        }

        [Test]
        public void ArmRw_qdaddeq()
        {
            BuildTest(0x01408e50);  // qdaddeq r8, r0, r0
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r8 = __signed_sat_32(r0 + __signed_sat_32(r0 *s 2))");
        }

        [Test]
        public void ArmRw_smlalbt()
        {
            BuildTest(0xE14090c0);  // smlalbt sb, r0, r0, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = (int16) r0 *s (int16) (r0 >> 16) + r9_r0");
        }

        [Test]
        public void ArmRw_swpb()
        {
            BuildTest(0xE1409190);  // swpb sb, r0, [r0]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = std::atomic_exchange<byte>(r0, Mem0[r0:byte])");
        }

        [Test]
        public void ArmRw_smlaltb()
        {
            BuildTest(0xE14091a0);  // smlaltb sb, r0, r0, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = (int16) (r0 >> 16) *s (int16) r1 + r9_r0");
        }

        [Test]
        public void ArmRw_strt()
        {
            BuildTest(0xE6247800);  // strt r7, [r4], -r0, lsl #16
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r4:word32] = r7",
                "2|L--|r4 = r4 - (r0 << 16)");
        }

        [Test]
        public void ArmRw_ldrtmi()
        {
            BuildTest(0x44340000);  // ldrtmi r0, [r4], #-0
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|T--|if (Test(GE,N)) branch 00100004",
                "2|L--|v5 = Mem0[r4:word32]",
                "3|L--|r4 = r4 + 0x00000000",
                "4|L--|r0 = v5");
        }

        [Test]
        public void ArmRw_smlalbb()
        {
            BuildTest(0xE1409280);  // smlalbb sb, r0, r0, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9_r0 = (int16) r0 *s (int16) r2 + r9_r0");
        }

        [Test]
        public void ArmRw_smlaltt()
        {
            BuildTest(0xE140abec);  // smlaltt sl, r0, ip, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10_r0 = (int16) (ip >> 16) *s (int16) (fp >> 16) + r10_r0");
        }

        [Test]
        public void ArmRw_ldrls_pc()
        {
            BuildTest(0x979FF103);   // ldrls\tpc,[pc,r3,lsl #2]
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGT,ZC)) branch 00100004",
                "2|T--|goto Mem0[0x00100008 + r3 * 4:word32]");
        }

        [Test]
        public void ArmRw_svc()
        {
            BuildTest(0xEF001234); // svc 0x1234
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__syscall(0x00001234)");
        }

        [Test]
        public void ArmRw_ldc()
        {
            BuildTest(0xEC344444); // svc 0x1234
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r4 = r4",
                "2|L--|v3 = Mem0[r4:word32]",
                "3|L--|p4 = __ldc(0x04, v3)");
        }

        [Test]
        public void ArmRw_ReadPC()
        {
            BuildTest(0xE08FE00E);  // add lr, pc, lr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = 0x00100008 + lr");
        }
    }
}
