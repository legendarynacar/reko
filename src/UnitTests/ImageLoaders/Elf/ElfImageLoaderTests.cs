﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Arch.X86;
using Decompiler.ImageLoaders.Elf;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Decompiler.Core.Configuration;
using Decompiler.Core.Services;

namespace Decompiler.UnitTests.ImageLoaders.Elf
{
    [TestFixture]
    public class ElfImageLoaderTests
    {
        // bswap 
        private byte[] rawImg = {
        #region Test ELF image
            0x7F, 0x45, 0x4C, 0x46, 0x01, 0x01, 0x01, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x02, 0x00, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00,  0xC0, 0x82, 0x04, 0x08, 0x34, 0x00, 0x00, 0x00,  
            0xAC, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x34, 0x00, 0x20, 0x00, 0x07, 0x00, 0x28, 0x00, 
            0x1C, 0x00, 0x19, 0x00, 0x06, 0x00, 0x00, 0x00,  0x34, 0x00, 0x00, 0x00, 0x34, 0x80, 0x04, 0x08, 
            0x34, 0x80, 0x04, 0x08, 0xE0, 0x00, 0x00, 0x00,  0xE0, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 
            0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  0x14, 0x01, 0x00, 0x00, 0x14, 0x81, 0x04, 0x08,
            0x14, 0x81, 0x04, 0x08, 0x13, 0x00, 0x00, 0x00,  0x13, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x04, 0x08,  
            0x00, 0x80, 0x04, 0x08, 0xA0, 0x04, 0x00, 0x00,  0xA0, 0x04, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,  
            0x00, 0x10, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0xA0, 0x04, 0x00, 0x00, 0xA0, 0x94, 0x04, 0x08,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x01, 0x00, 0x00,  0x04, 0x01, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x00, 0x10, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  0xB4, 0x04, 0x00, 0x00, 0xB4, 0x94, 0x04, 0x08,  
            0xB4, 0x94, 0x04, 0x08, 0xC8, 0x00, 0x00, 0x00,  0xC8, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x28, 0x01, 0x00, 0x00, 0x28, 0x81, 0x04, 0x08,  
            0x28, 0x81, 0x04, 0x08, 0x20, 0x00, 0x00, 0x00,  0x20, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x51, 0xE5, 0x74, 0x64,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x2F, 0x6C, 0x69, 0x62,  0x2F, 0x6C, 0x64, 0x2D, 0x6C, 0x69, 0x6E, 0x75,  
            0x78, 0x2E, 0x73, 0x6F, 0x2E, 0x32, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x47, 0x4E, 0x55, 0x00,  0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x02, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,  0x03, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x44, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xEF, 0x00, 0x00, 0x00,  
            0x12, 0x00, 0x00, 0x00, 0x2E, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x39, 0x00, 0x00, 0x00,  
            0x12, 0x00, 0x00, 0x00, 0x35, 0x00, 0x00, 0x00,  0x88, 0x84, 0x04, 0x08, 0x04, 0x00, 0x00, 0x00,  
            0x11, 0x00, 0x0E, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x20, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x20, 0x00, 0x00, 0x00, 0x00, 0x5F, 0x4A, 0x76,  0x5F, 0x52, 0x65, 0x67, 0x69, 0x73, 0x74, 0x65,  
            0x72, 0x43, 0x6C, 0x61, 0x73, 0x73, 0x65, 0x73,  0x00, 0x5F, 0x5F, 0x67, 0x6D, 0x6F, 0x6E, 0x5F,  
            0x73, 0x74, 0x61, 0x72, 0x74, 0x5F, 0x5F, 0x00,  0x6C, 0x69, 0x62, 0x63, 0x2E, 0x73, 0x6F, 0x2E,  
            0x36, 0x00, 0x70, 0x72, 0x69, 0x6E, 0x74, 0x66,  0x00, 0x5F, 0x49, 0x4F, 0x5F, 0x73, 0x74, 0x64,  
            0x69, 0x6E, 0x5F, 0x75, 0x73, 0x65, 0x64, 0x00,  0x5F, 0x5F, 0x6C, 0x69, 0x62, 0x63, 0x5F, 0x73,  
            0x74, 0x61, 0x72, 0x74, 0x5F, 0x6D, 0x61, 0x69,  0x6E, 0x00, 0x47, 0x4C, 0x49, 0x42, 0x43, 0x5F,  
            0x32, 0x2E, 0x30, 0x00, 0x00, 0x00, 0x02, 0x00,  0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x01, 0x00, 0x24, 0x00, 0x00, 0x00,  0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x10, 0x69, 0x69, 0x0D, 0x00, 0x00, 0x02, 0x00,  0x56, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x7C, 0x95, 0x04, 0x08, 0x06, 0x05, 0x00, 0x00,  0x8C, 0x95, 0x04, 0x08, 0x07, 0x01, 0x00, 0x00,  
            0x90, 0x95, 0x04, 0x08, 0x07, 0x02, 0x00, 0x00,  0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08, 0xE8, 0x61,  
            0x00, 0x00, 0x00, 0xE8, 0xBC, 0x00, 0x00, 0x00,  0xE8, 0xB7, 0x01, 0x00, 0x00, 0xC9, 0xC3, 0x00,  
            0xFF, 0x35, 0x84, 0x95, 0x04, 0x08, 0xFF, 0x25,  0x88, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  
            0xFF, 0x25, 0x8C, 0x95, 0x04, 0x08, 0x68, 0x00,  0x00, 0x00, 0x00, 0xE9, 0xE0, 0xFF, 0xFF, 0xFF,  
            0xFF, 0x25, 0x90, 0x95, 0x04, 0x08, 0x68, 0x08,  0x00, 0x00, 0x00, 0xE9, 0xD0, 0xFF, 0xFF, 0xFF,  
            0x31, 0xED, 0x5E, 0x89, 0xE1, 0x83, 0xE4, 0xF0,  0x50, 0x54, 0x52, 0x68, 0x00, 0x84, 0x04, 0x08,  
            0x68, 0xB8, 0x83, 0x04, 0x08, 0x51, 0x56, 0x68,  0x7A, 0x83, 0x04, 0x08, 0xE8, 0xBF, 0xFF, 0xFF,  
            0xFF, 0xF4, 0x90, 0x90, 0x55, 0x89, 0xE5, 0x53,  0xE8, 0x00, 0x00, 0x00, 0x00, 0x5B, 0x81, 0xC3,  
            0x93, 0x12, 0x00, 0x00, 0x50, 0x8B, 0x83, 0xFC,  0xFF, 0xFF, 0xFF, 0x85, 0xC0, 0x74, 0x02, 0xFF,  
            0xD0, 0x8B, 0x5D, 0xFC, 0xC9, 0xC3, 0x90, 0x90,  0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08, 0x80, 0x3D,  
            0xA0, 0x95, 0x04, 0x08, 0x00, 0x75, 0x29, 0xA1,  0x9C, 0x95, 0x04, 0x08, 0x8B, 0x10, 0x85, 0xD2,  
            0x74, 0x17, 0x89, 0xF6, 0x83, 0xC0, 0x04, 0xA3,  0x9C, 0x95, 0x04, 0x08, 0xFF, 0xD2, 0xA1, 0x9C,  
            0x95, 0x04, 0x08, 0x8B, 0x10, 0x85, 0xD2, 0x75,  0xEB, 0xC6, 0x05, 0xA0, 0x95, 0x04, 0x08, 0x01,  
            0xC9, 0xC3, 0x89, 0xF6, 0x55, 0x89, 0xE5, 0x83,  0xEC, 0x08, 0xA1, 0xB0, 0x94, 0x04, 0x08, 0x85,  
            0xC0, 0x74, 0x19, 0xB8, 0x00, 0x00, 0x00, 0x00,  0x85, 0xC0, 0x74, 0x10, 0x83, 0xEC, 0x0C, 0x68,  
            0xB0, 0x94, 0x04, 0x08, 0xFF, 0xD0, 0x83, 0xC4,  0x10, 0x8D, 0x76, 0x00, 0xC9, 0xC3, 0x90, 0x90,  
            0x55, 0x89, 0xE5, 0x8B, 0x45, 0x08, 0x0F, 0xC8,  0xC9, 0xC3, 0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08,  
            0x83, 0xE4, 0xF0, 0xB8, 0x00, 0x00, 0x00, 0x00,  0x29, 0xC4, 0x83, 0xEC, 0x0C, 0x68, 0x78, 0x56,  
            0x34, 0x12, 0xE8, 0xD9, 0xFF, 0xFF, 0xFF, 0x83,  0xC4, 0x10, 0x89, 0x45, 0xFC, 0x83, 0xEC, 0x08,  
            0xFF, 0x75, 0xFC, 0x68, 0x8C, 0x84, 0x04, 0x08,  0xE8, 0x03, 0xFF, 0xFF, 0xFF, 0x83, 0xC4, 0x10,  
            0xB8, 0x00, 0x00, 0x00, 0x00, 0xC9, 0xC3, 0x90,  0x55, 0x89, 0xE5, 0x57, 0x56, 0x53, 0x83, 0xEC,  
            0x0C, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5B, 0x81,  0xC3, 0xBA, 0x11, 0x00, 0x00, 0xE8, 0xA6, 0xFE,  
            0xFF, 0xFF, 0x8D, 0x93, 0x20, 0xFF, 0xFF, 0xFF,  0x8D, 0x8B, 0x20, 0xFF, 0xFF, 0xFF, 0x29, 0xCA,  
            0x31, 0xF6, 0xC1, 0xFA, 0x02, 0x39, 0xD6, 0x73,  0x0F, 0x89, 0xD7, 0x90, 0xFF, 0x94, 0xB3, 0x20,  
            0xFF, 0xFF, 0xFF, 0x46, 0x39, 0xFE, 0x72, 0xF4,  0x83, 0xC4, 0x0C, 0x5B, 0x5E, 0x5F, 0xC9, 0xC3,  
            0x55, 0x89, 0xE5, 0x56, 0x53, 0xE8, 0x00, 0x00,  0x00, 0x00, 0x5B, 0x81, 0xC3, 0x76, 0x11, 0x00,  
            0x00, 0x8D, 0x8B, 0x20, 0xFF, 0xFF, 0xFF, 0x8D,  0x83, 0x20, 0xFF, 0xFF, 0xFF, 0x29, 0xC1, 0xC1,  
            0xF9, 0x02, 0x85, 0xC9, 0x8D, 0x71, 0xFF, 0x75,  0x0B, 0xE8, 0x3A, 0x00, 0x00, 0x00, 0x5B, 0x5E,  
            0xC9, 0xC3, 0x89, 0xF6, 0xFF, 0x94, 0xB3, 0x20,  0xFF, 0xFF, 0xFF, 0x89, 0xF2, 0x4E, 0x85, 0xD2,  
            0x75, 0xF2, 0xEB, 0xE5, 0x55, 0x89, 0xE5, 0x53,  0x52, 0xA1, 0xA0, 0x94, 0x04, 0x08, 0x83, 0xF8,  
            0xFF, 0xBB, 0xA0, 0x94, 0x04, 0x08, 0x74, 0x0C,  0x83, 0xEB, 0x04, 0xFF, 0xD0, 0x8B, 0x03, 0x83,  
            0xF8, 0xFF, 0x75, 0xF4, 0x58, 0x5B, 0xC9, 0xC3,  0x55, 0x89, 0xE5, 0x53, 0xE8, 0x00, 0x00, 0x00,  
            0x00, 0x5B, 0x81, 0xC3, 0x0F, 0x11, 0x00, 0x00,  0x52, 0xE8, 0x8A, 0xFE, 0xFF, 0xFF, 0x8B, 0x5D,  
            0xFC, 0xC9, 0xC3, 0x00, 0x03, 0x00, 0x00, 0x00,  0x01, 0x00, 0x02, 0x00, 0x4F, 0x75, 0x74, 0x70,  
            0x75, 0x74, 0x20, 0x69, 0x73, 0x20, 0x25, 0x78,  0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,  0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x24, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00,  
            0x78, 0x82, 0x04, 0x08, 0x0D, 0x00, 0x00, 0x00,  0x68, 0x84, 0x04, 0x08, 0x04, 0x00, 0x00, 0x00,  
            0x48, 0x81, 0x04, 0x08, 0x05, 0x00, 0x00, 0x00,  0xD4, 0x81, 0x04, 0x08, 0x06, 0x00, 0x00, 0x00,  
            0x74, 0x81, 0x04, 0x08, 0x0A, 0x00, 0x00, 0x00,  0x60, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00,  
            0x10, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  
            0x80, 0x95, 0x04, 0x08, 0x02, 0x00, 0x00, 0x00,  0x10, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00,  
            0x11, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00,  0x68, 0x82, 0x04, 0x08, 0x11, 0x00, 0x00, 0x00,  
            0x60, 0x82, 0x04, 0x08, 0x12, 0x00, 0x00, 0x00,  0x08, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00,  
            0x08, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0x6F,  0x40, 0x82, 0x04, 0x08, 0xFF, 0xFF, 0xFF, 0x6F,  
            0x01, 0x00, 0x00, 0x00, 0xF0, 0xFF, 0xFF, 0x6F,  0x34, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xB4, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xA6, 0x82, 0x04, 0x08,  
            0xB6, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xAC, 0x94, 0x04, 0x08,  
            0x00, 0x47, 0x43, 0x43, 0x3A, 0x20, 0x28, 0x47,  0x4E, 0x55, 0x29, 0x20, 0x33, 0x2E, 0x33, 0x2E,  
            0x33, 0x20, 0x32, 0x30, 0x30, 0x34, 0x30, 0x34,  0x31, 0x32, 0x20, 0x28, 0x52, 0x65, 0x64, 0x20,  
            0x48, 0x61, 0x74, 0x20, 0x4C, 0x69, 0x6E, 0x75,  0x78, 0x20, 0x33, 0x2E, 0x33, 0x2E, 0x33, 0x2D,  
            0x37, 0x29, 0x00, 0x00, 0x47, 0x43, 0x43, 0x3A,  0x20, 0x28, 0x47, 0x4E, 0x55, 0x29, 0x20, 0x33,  
            0x2E, 0x33, 0x2E, 0x33, 0x20, 0x32, 0x30, 0x30,  0x34, 0x30, 0x34, 0x31, 0x32, 0x20, 0x28, 0x52,  
            0x65, 0x64, 0x20, 0x48, 0x61, 0x74, 0x20, 0x4C,  0x69, 0x6E, 0x75, 0x78, 0x20, 0x33, 0x2E, 0x33,  
            0x2E, 0x33, 0x2D, 0x37, 0x29, 0x00, 0x00, 0x47,  0x43, 0x43, 0x3A, 0x20, 0x28, 0x47, 0x4E, 0x55,  
            0x29, 0x20, 0x33, 0x2E, 0x33, 0x2E, 0x33, 0x20,  0x32, 0x30, 0x30, 0x34, 0x30, 0x34, 0x31, 0x32,  
            0x20, 0x28, 0x52, 0x65, 0x64, 0x20, 0x48, 0x61,  0x74, 0x20, 0x4C, 0x69, 0x6E, 0x75, 0x78, 0x20,  
            0x33, 0x2E, 0x33, 0x2E, 0x33, 0x2D, 0x37, 0x29,  0x00, 0x00, 0x47, 0x43, 0x43, 0x3A, 0x20, 0x28,  
            0x47, 0x4E, 0x55, 0x29, 0x20, 0x33, 0x2E, 0x33,  0x2E, 0x33, 0x20, 0x32, 0x30, 0x30, 0x34, 0x30,  
            0x34, 0x31, 0x32, 0x20, 0x28, 0x52, 0x65, 0x64,  0x20, 0x48, 0x61, 0x74, 0x20, 0x4C, 0x69, 0x6E,  
            0x75, 0x78, 0x20, 0x33, 0x2E, 0x33, 0x2E, 0x33,  0x2D, 0x37, 0x29, 0x00, 0x00, 0x47, 0x43, 0x43,  
            0x3A, 0x20, 0x28, 0x47, 0x4E, 0x55, 0x29, 0x20,  0x33, 0x2E, 0x33, 0x2E, 0x33, 0x20, 0x32, 0x30,  
            0x30, 0x34, 0x30, 0x34, 0x31, 0x32, 0x20, 0x28,  0x52, 0x65, 0x64, 0x20, 0x48, 0x61, 0x74, 0x20,  
            0x4C, 0x69, 0x6E, 0x75, 0x78, 0x20, 0x33, 0x2E,  0x33, 0x2E, 0x33, 0x2D, 0x37, 0x29, 0x00, 0x00,  
            0x47, 0x43, 0x43, 0x3A, 0x20, 0x28, 0x47, 0x4E,  0x55, 0x29, 0x20, 0x33, 0x2E, 0x33, 0x2E, 0x33,  
            0x20, 0x32, 0x30, 0x30, 0x34, 0x30, 0x34, 0x31,  0x32, 0x20, 0x28, 0x52, 0x65, 0x64, 0x20, 0x48,  
            0x61, 0x74, 0x20, 0x4C, 0x69, 0x6E, 0x75, 0x78,  0x20, 0x33, 0x2E, 0x33, 0x2E, 0x33, 0x2D, 0x37,  
            0x29, 0x00, 0x00, 0x2E, 0x73, 0x79, 0x6D, 0x74,  0x61, 0x62, 0x00, 0x2E, 0x73, 0x74, 0x72, 0x74,  
            0x61, 0x62, 0x00, 0x2E, 0x73, 0x68, 0x73, 0x74,  0x72, 0x74, 0x61, 0x62, 0x00, 0x2E, 0x69, 0x6E,  
            0x74, 0x65, 0x72, 0x70, 0x00, 0x2E, 0x6E, 0x6F,  0x74, 0x65, 0x2E, 0x41, 0x42, 0x49, 0x2D, 0x74,  
            0x61, 0x67, 0x00, 0x2E, 0x68, 0x61, 0x73, 0x68,  0x00, 0x2E, 0x64, 0x79, 0x6E, 0x73, 0x79, 0x6D,  
            0x00, 0x2E, 0x64, 0x79, 0x6E, 0x73, 0x74, 0x72,  0x00, 0x2E, 0x67, 0x6E, 0x75, 0x2E, 0x76, 0x65,  
            0x72, 0x73, 0x69, 0x6F, 0x6E, 0x00, 0x2E, 0x67,  0x6E, 0x75, 0x2E, 0x76, 0x65, 0x72, 0x73, 0x69,  
            0x6F, 0x6E, 0x5F, 0x72, 0x00, 0x2E, 0x72, 0x65,  0x6C, 0x2E, 0x64, 0x79, 0x6E, 0x00, 0x2E, 0x72,  
            0x65, 0x6C, 0x2E, 0x70, 0x6C, 0x74, 0x00, 0x2E,  0x69, 0x6E, 0x69, 0x74, 0x00, 0x2E, 0x74, 0x65,  
            0x78, 0x74, 0x00, 0x2E, 0x66, 0x69, 0x6E, 0x69,  0x00, 0x2E, 0x72, 0x6F, 0x64, 0x61, 0x74, 0x61,  
            0x00, 0x2E, 0x65, 0x68, 0x5F, 0x66, 0x72, 0x61,  0x6D, 0x65, 0x00, 0x2E, 0x63, 0x74, 0x6F, 0x72,  
            0x73, 0x00, 0x2E, 0x64, 0x74, 0x6F, 0x72, 0x73,  0x00, 0x2E, 0x6A, 0x63, 0x72, 0x00, 0x2E, 0x64,  
            0x79, 0x6E, 0x61, 0x6D, 0x69, 0x63, 0x00, 0x2E,  0x67, 0x6F, 0x74, 0x00, 0x2E, 0x67, 0x6F, 0x74,  
            0x2E, 0x70, 0x6C, 0x74, 0x00, 0x2E, 0x64, 0x61,  0x74, 0x61, 0x00, 0x2E, 0x62, 0x73, 0x73, 0x00,  
            0x2E, 0x63, 0x6F, 0x6D, 0x6D, 0x65, 0x6E, 0x74,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x14, 0x81, 0x04, 0x08, 0x14, 0x01, 0x00, 0x00,  0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00,  
            0x07, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  0x28, 0x81, 0x04, 0x08, 0x28, 0x01, 0x00, 0x00,  
            0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00,  0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x48, 0x81, 0x04, 0x08, 0x48, 0x01, 0x00, 0x00,  0x2C, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0x37, 0x00, 0x00, 0x00,  
            0x0B, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  0x74, 0x81, 0x04, 0x08, 0x74, 0x01, 0x00, 0x00,  
            0x60, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x10, 0x00, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00,  0x03, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0xD4, 0x81, 0x04, 0x08, 0xD4, 0x01, 0x00, 0x00,  0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x47, 0x00, 0x00, 0x00,  
            0xFF, 0xFF, 0xFF, 0x6F, 0x02, 0x00, 0x00, 0x00,  0x34, 0x82, 0x04, 0x08, 0x34, 0x02, 0x00, 0x00,  
            0x0C, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x02, 0x00, 0x00, 0x00, 0x54, 0x00, 0x00, 0x00,  0xFE, 0xFF, 0xFF, 0x6F, 0x02, 0x00, 0x00, 0x00,  
            0x40, 0x82, 0x04, 0x08, 0x40, 0x02, 0x00, 0x00,  0x20, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x63, 0x00, 0x00, 0x00,  
            0x09, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  0x60, 0x82, 0x04, 0x08, 0x60, 0x02, 0x00, 0x00,  
            0x08, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x08, 0x00, 0x00, 0x00, 0x6C, 0x00, 0x00, 0x00,  0x09, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x68, 0x82, 0x04, 0x08, 0x68, 0x02, 0x00, 0x00,  0x10, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x0B, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x08, 0x00, 0x00, 0x00, 0x75, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  0x78, 0x82, 0x04, 0x08, 0x78, 0x02, 0x00, 0x00,  
            0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x90, 0x82, 0x04, 0x08, 0x90, 0x02, 0x00, 0x00,  0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0x7B, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  0xC0, 0x82, 0x04, 0x08, 0xC0, 0x02, 0x00, 0x00,  
            0xA8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x81, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,  
            0x68, 0x84, 0x04, 0x08, 0x68, 0x04, 0x00, 0x00,  0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x87, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  0x84, 0x84, 0x04, 0x08, 0x84, 0x04, 0x00, 0x00,  
            0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x8F, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,  
            0x9C, 0x84, 0x04, 0x08, 0x9C, 0x04, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x99, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  0xA0, 0x94, 0x04, 0x08, 0xA0, 0x04, 0x00, 0x00,  
            0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  
            0xA8, 0x94, 0x04, 0x08, 0xA8, 0x04, 0x00, 0x00,  0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xA7, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  0xB0, 0x94, 0x04, 0x08, 0xB0, 0x04, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0xAC, 0x00, 0x00, 0x00,  0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  
            0xB4, 0x94, 0x04, 0x08, 0xB4, 0x04, 0x00, 0x00,  0xC8, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x08, 0x00, 0x00, 0x00, 0xB5, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  0x7C, 0x95, 0x04, 0x08, 0x7C, 0x05, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x04, 0x00, 0x00, 0x00, 0xBA, 0x00, 0x00, 0x00,  0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  
            0x80, 0x95, 0x04, 0x08, 0x80, 0x05, 0x00, 0x00,  0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0xC3, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  0x94, 0x95, 0x04, 0x08, 0x94, 0x05, 0x00, 0x00,  
            0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0xC9, 0x00, 0x00, 0x00,  0x08, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,  
            0xA0, 0x95, 0x04, 0x08, 0xA0, 0x05, 0x00, 0x00,  0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xCE, 0x00, 0x00, 0x00,  
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0xA0, 0x05, 0x00, 0x00,  
            0x32, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00,  0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0xD2, 0x06, 0x00, 0x00,  0xD7, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x0C, 0x0C, 0x00, 0x00,  
            0x70, 0x04, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00,  0x2C, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,  
            0x10, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00,  0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x7C, 0x10, 0x00, 0x00,  0x3D, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x14, 0x81, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x28, 0x81, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x48, 0x81, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x74, 0x81, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xD4, 0x81, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x34, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x40, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x60, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x68, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x78, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x90, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xC0, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x68, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x84, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x9C, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xA8, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xB0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xB4, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x7C, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x80, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x94, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0xA0, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x03, 0x00, 0x1B, 0x00, 0x01, 0x00, 0x00, 0x00,  
            0xE4, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x02, 0x00, 0x0C, 0x00, 0x11, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x04, 0x00, 0xF1, 0xFF, 0x1C, 0x00, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x10, 0x00, 0x2A, 0x00, 0x00, 0x00,  
            0xA8, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x11, 0x00, 0x38, 0x00, 0x00, 0x00,  
            0xB0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x12, 0x00, 0x45, 0x00, 0x00, 0x00,  
            0x9C, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x16, 0x00, 0x49, 0x00, 0x00, 0x00,  
            0xA0, 0x95, 0x04, 0x08, 0x01, 0x00, 0x00, 0x00,  0x01, 0x00, 0x17, 0x00, 0x55, 0x00, 0x00, 0x00,  
            0x08, 0x83, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x02, 0x00, 0x0C, 0x00, 0x6B, 0x00, 0x00, 0x00,  
            0x44, 0x83, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x02, 0x00, 0x0C, 0x00, 0x11, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x04, 0x00, 0xF1, 0xFF, 0x77, 0x00, 0x00, 0x00,  
            0xA4, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x10, 0x00, 0x84, 0x00, 0x00, 0x00,  
            0xAC, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x11, 0x00, 0x91, 0x00, 0x00, 0x00,  
            0x9C, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x0F, 0x00, 0x9F, 0x00, 0x00, 0x00,  
            0xB0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x01, 0x00, 0x12, 0x00, 0xAB, 0x00, 0x00, 0x00,  
            0x44, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x02, 0x00, 0x0C, 0x00, 0xC1, 0x00, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x04, 0x00, 0xF1, 0xFF, 0xC9, 0x00, 0x00, 0x00,  
            0xB4, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x11, 0x00, 0x13, 0x00, 0xD2, 0x00, 0x00, 0x00,  
            0x84, 0x84, 0x04, 0x08, 0x04, 0x00, 0x00, 0x00,  0x11, 0x00, 0x0E, 0x00, 0xD9, 0x00, 0x00, 0x00,  
            0x70, 0x83, 0x04, 0x08, 0x0A, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0C, 0x00, 0xDF, 0x00, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0xF0, 0x00, 0x00, 0x00,  
            0x98, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x11, 0x02, 0x16, 0x00, 0xFD, 0x00, 0x00, 0x00,  
            0x00, 0x84, 0x04, 0x08, 0x44, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0C, 0x00, 0x0D, 0x01, 0x00, 0x00,  
            0x78, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0A, 0x00, 0x13, 0x01, 0x00, 0x00,  
            0xC0, 0x82, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0C, 0x00, 0x1A, 0x01, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0x2D, 0x01, 0x00, 0x00,  
            0xB8, 0x83, 0x04, 0x08, 0x48, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0C, 0x00, 0x3D, 0x01, 0x00, 0x00,  
            0xA0, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x00, 0xF1, 0xFF, 0x49, 0x01, 0x00, 0x00,  
            0x7A, 0x83, 0x04, 0x08, 0x3D, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0C, 0x00, 0x4E, 0x01, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0xEF, 0x00, 0x00, 0x00,  0x12, 0x00, 0x00, 0x00, 0x6B, 0x01, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0x7C, 0x01, 0x00, 0x00,  
            0x94, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x20, 0x00, 0x16, 0x00, 0x87, 0x01, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x39, 0x00, 0x00, 0x00,  0x12, 0x00, 0x00, 0x00, 0x99, 0x01, 0x00, 0x00,  
            0x68, 0x84, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x12, 0x00, 0x0D, 0x00, 0x9F, 0x01, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0xB3, 0x01, 0x00, 0x00,  
            0xA0, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x00, 0xF1, 0xFF, 0xBA, 0x01, 0x00, 0x00,  
            0x80, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x11, 0x00, 0x15, 0x00, 0xD0, 0x01, 0x00, 0x00,  
            0xA4, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x00, 0xF1, 0xFF, 0xD5, 0x01, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0xE8, 0x01, 0x00, 0x00,  
            0x88, 0x84, 0x04, 0x08, 0x04, 0x00, 0x00, 0x00,  0x11, 0x00, 0x0E, 0x00, 0xF7, 0x01, 0x00, 0x00,  
            0x94, 0x95, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x00, 0x16, 0x00, 0x04, 0x02, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x20, 0x00, 0x00, 0x00, 0x18, 0x02, 0x00, 0x00,  
            0xA0, 0x94, 0x04, 0x08, 0x00, 0x00, 0x00, 0x00,  0x10, 0x02, 0xF1, 0xFF, 0x2E, 0x02, 0x00, 0x00,  
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  0x20, 0x00, 0x00, 0x00, 0x00, 0x63, 0x61, 0x6C,  
            0x6C, 0x5F, 0x67, 0x6D, 0x6F, 0x6E, 0x5F, 0x73,  0x74, 0x61, 0x72, 0x74, 0x00, 0x63, 0x72, 0x74,  
            0x73, 0x74, 0x75, 0x66, 0x66, 0x2E, 0x63, 0x00,  0x5F, 0x5F, 0x43, 0x54, 0x4F, 0x52, 0x5F, 0x4C,  
            0x49, 0x53, 0x54, 0x5F, 0x5F, 0x00, 0x5F, 0x5F,  0x44, 0x54, 0x4F, 0x52, 0x5F, 0x4C, 0x49, 0x53,  
            0x54, 0x5F, 0x5F, 0x00, 0x5F, 0x5F, 0x4A, 0x43,  0x52, 0x5F, 0x4C, 0x49, 0x53, 0x54, 0x5F, 0x5F,  
            0x00, 0x70, 0x2E, 0x30, 0x00, 0x63, 0x6F, 0x6D,  0x70, 0x6C, 0x65, 0x74, 0x65, 0x64, 0x2E, 0x31,  
            0x00, 0x5F, 0x5F, 0x64, 0x6F, 0x5F, 0x67, 0x6C,  0x6F, 0x62, 0x61, 0x6C, 0x5F, 0x64, 0x74, 0x6F,  
            0x72, 0x73, 0x5F, 0x61, 0x75, 0x78, 0x00, 0x66,  0x72, 0x61, 0x6D, 0x65, 0x5F, 0x64, 0x75, 0x6D,  
            0x6D, 0x79, 0x00, 0x5F, 0x5F, 0x43, 0x54, 0x4F,  0x52, 0x5F, 0x45, 0x4E, 0x44, 0x5F, 0x5F, 0x00,  
            0x5F, 0x5F, 0x44, 0x54, 0x4F, 0x52, 0x5F, 0x45,  0x4E, 0x44, 0x5F, 0x5F, 0x00, 0x5F, 0x5F, 0x46,  
            0x52, 0x41, 0x4D, 0x45, 0x5F, 0x45, 0x4E, 0x44,  0x5F, 0x5F, 0x00, 0x5F, 0x5F, 0x4A, 0x43, 0x52,  
            0x5F, 0x45, 0x4E, 0x44, 0x5F, 0x5F, 0x00, 0x5F,  0x5F, 0x64, 0x6F, 0x5F, 0x67, 0x6C, 0x6F, 0x62,  
            0x61, 0x6C, 0x5F, 0x63, 0x74, 0x6F, 0x72, 0x73,  0x5F, 0x61, 0x75, 0x78, 0x00, 0x62, 0x73, 0x77,  
            0x61, 0x70, 0x2E, 0x63, 0x00, 0x5F, 0x44, 0x59,  0x4E, 0x41, 0x4D, 0x49, 0x43, 0x00, 0x5F, 0x66,  
            0x70, 0x5F, 0x68, 0x77, 0x00, 0x62, 0x73, 0x77,  0x61, 0x70, 0x00, 0x5F, 0x5F, 0x66, 0x69, 0x6E,  
            0x69, 0x5F, 0x61, 0x72, 0x72, 0x61, 0x79, 0x5F,  0x65, 0x6E, 0x64, 0x00, 0x5F, 0x5F, 0x64, 0x73,  
            0x6F, 0x5F, 0x68, 0x61, 0x6E, 0x64, 0x6C, 0x65,  0x00, 0x5F, 0x5F, 0x6C, 0x69, 0x62, 0x63, 0x5F,  
            0x63, 0x73, 0x75, 0x5F, 0x66, 0x69, 0x6E, 0x69,  0x00, 0x5F, 0x69, 0x6E, 0x69, 0x74, 0x00, 0x5F,  
            0x73, 0x74, 0x61, 0x72, 0x74, 0x00, 0x5F, 0x5F,  0x66, 0x69, 0x6E, 0x69, 0x5F, 0x61, 0x72, 0x72,  
            0x61, 0x79, 0x5F, 0x73, 0x74, 0x61, 0x72, 0x74,  0x00, 0x5F, 0x5F, 0x6C, 0x69, 0x62, 0x63, 0x5F,  
            0x63, 0x73, 0x75, 0x5F, 0x69, 0x6E, 0x69, 0x74,  0x00, 0x5F, 0x5F, 0x62, 0x73, 0x73, 0x5F, 0x73,  
            0x74, 0x61, 0x72, 0x74, 0x00, 0x6D, 0x61, 0x69,  0x6E, 0x00, 0x5F, 0x5F, 0x6C, 0x69, 0x62, 0x63,  
            0x5F, 0x73, 0x74, 0x61, 0x72, 0x74, 0x5F, 0x6D,  0x61, 0x69, 0x6E, 0x40, 0x40, 0x47, 0x4C, 0x49,  
            0x42, 0x43, 0x5F, 0x32, 0x2E, 0x30, 0x00, 0x5F,  0x5F, 0x69, 0x6E, 0x69, 0x74, 0x5F, 0x61, 0x72,  
            0x72, 0x61, 0x79, 0x5F, 0x65, 0x6E, 0x64, 0x00,  0x64, 0x61, 0x74, 0x61, 0x5F, 0x73, 0x74, 0x61,  
            0x72, 0x74, 0x00, 0x70, 0x72, 0x69, 0x6E, 0x74,  0x66, 0x40, 0x40, 0x47, 0x4C, 0x49, 0x42, 0x43,  
            0x5F, 0x32, 0x2E, 0x30, 0x00, 0x5F, 0x66, 0x69,  0x6E, 0x69, 0x00, 0x5F, 0x5F, 0x70, 0x72, 0x65,  
            0x69, 0x6E, 0x69, 0x74, 0x5F, 0x61, 0x72, 0x72,  0x61, 0x79, 0x5F, 0x65, 0x6E, 0x64, 0x00, 0x5F,  
            0x65, 0x64, 0x61, 0x74, 0x61, 0x00, 0x5F, 0x47,  0x4C, 0x4F, 0x42, 0x41, 0x4C, 0x5F, 0x4F, 0x46,  
            0x46, 0x53, 0x45, 0x54, 0x5F, 0x54, 0x41, 0x42,  0x4C, 0x45, 0x5F, 0x00, 0x5F, 0x65, 0x6E, 0x64,  
            0x00, 0x5F, 0x5F, 0x69, 0x6E, 0x69, 0x74, 0x5F,  0x61, 0x72, 0x72, 0x61, 0x79, 0x5F, 0x73, 0x74,  
            0x61, 0x72, 0x74, 0x00, 0x5F, 0x49, 0x4F, 0x5F,  0x73, 0x74, 0x64, 0x69, 0x6E, 0x5F, 0x75, 0x73,  
            0x65, 0x64, 0x00, 0x5F, 0x5F, 0x64, 0x61, 0x74,  0x61, 0x5F, 0x73, 0x74, 0x61, 0x72, 0x74, 0x00,  
            0x5F, 0x4A, 0x76, 0x5F, 0x52, 0x65, 0x67, 0x69,  0x73, 0x74, 0x65, 0x72, 0x43, 0x6C, 0x61, 0x73,  
            0x73, 0x65, 0x73, 0x00, 0x5F, 0x5F, 0x70, 0x72,  0x65, 0x69, 0x6E, 0x69, 0x74, 0x5F, 0x61, 0x72,  
            0x72, 0x61, 0x79, 0x5F, 0x73, 0x74, 0x61, 0x72,  0x74, 0x00, 0x5F, 0x5F, 0x67, 0x6D, 0x6F, 0x6E,  
            0x5F, 0x73, 0x74, 0x61, 0x72, 0x74, 0x5F, 0x5F,  0x00
        #endregion
        };

        private MockRepository mr;
        private IProcessorArchitecture arch;
        private IServiceProvider services;
        private IDecompilerConfigurationService dcSvc;
        private ITypeLibraryLoaderService tlSvc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = mr.Stub<IProcessorArchitecture>();
            this.services = mr.Stub<IServiceProvider>();
            this.dcSvc = mr.Stub<IDecompilerConfigurationService>();
            this.tlSvc = mr.Stub<ITypeLibraryLoaderService>(); 
            services.Stub(s => s.GetService(typeof(IDecompilerConfigurationService))).Return(dcSvc);
            services.Stub(s => s.GetService(typeof(ITypeLibraryLoaderService))).Return(tlSvc);
            dcSvc.Stub(d => d.GetArchitecture("x86-protected-32")).Return(arch);
            dcSvc.Stub(d => d.GetEnvironment("elf-neutral")).Return(new OperatingEnvironmentElement());
        }

        // 336 - 37
        [Test]
        public void EIL_Load()
        {
            mr.ReplayAll();

            var el = new ElfImageLoader(services, "foo", rawImg);
            var lr = el.Load(new Address(0));
            Assert.AreSame(arch, lr.Architecture);
        }

        [Test]
        public void EIL_LoadStringTable()
        {
            mr.ReplayAll();

            var el = new ElfImageLoader(services, "foo", rawImg);
            el.Load(new Address(0));
            Assert.AreEqual(".symtab", el.GetSectionName(1));
        }

        [Test]
        public void EIL_LoadSections()
        {
            mr.ReplayAll();

            var el = new ElfImageLoader(services, "foo", rawImg);
            el.Load(new Address(0));

            Assert.AreEqual("", el.GetSectionName(el.SectionHeaders[0].sh_name));
            Assert.AreEqual(".interp", el.GetSectionName(el.SectionHeaders[1].sh_name));
            Assert.AreEqual(".note.ABI-tag", el.GetSectionName(el.SectionHeaders[2].sh_name));
            Assert.AreEqual(".hash", el.GetSectionName(el.SectionHeaders[3].sh_name));
            Assert.AreEqual(".dynsym", el.GetSectionName(el.SectionHeaders[4].sh_name));
            Assert.AreEqual(".dynstr", el.GetSectionName(el.SectionHeaders[5].sh_name));
            Assert.AreEqual(".gnu.version", el.GetSectionName(el.SectionHeaders[6].sh_name));
            Assert.AreEqual(".gnu.version_r", el.GetSectionName(el.SectionHeaders[7].sh_name));
            Assert.AreEqual(".rel.dyn", el.GetSectionName(el.SectionHeaders[8].sh_name));
            Assert.AreEqual(".rel.plt", el.GetSectionName(el.SectionHeaders[9].sh_name));
            Assert.AreEqual(".init", el.GetSectionName(el.SectionHeaders[10].sh_name));
            Assert.AreEqual(".plt", el.GetSectionName(el.SectionHeaders[11].sh_name));
            Assert.AreEqual(".text", el.GetSectionName(el.SectionHeaders[12].sh_name));
            Assert.AreEqual(".fini", el.GetSectionName(el.SectionHeaders[13].sh_name));
            Assert.AreEqual(".rodata", el.GetSectionName(el.SectionHeaders[14].sh_name));
            Assert.AreEqual(".eh_frame", el.GetSectionName(el.SectionHeaders[15].sh_name));
            Assert.AreEqual(".ctors", el.GetSectionName(el.SectionHeaders[16].sh_name));
            Assert.AreEqual(".dtors", el.GetSectionName(el.SectionHeaders[17].sh_name));
            Assert.AreEqual(".jcr", el.GetSectionName(el.SectionHeaders[18].sh_name));
            Assert.AreEqual(".dynamic", el.GetSectionName(el.SectionHeaders[19].sh_name));
            Assert.AreEqual(".got", el.GetSectionName(el.SectionHeaders[20].sh_name));
            Assert.AreEqual(".got.plt", el.GetSectionName(el.SectionHeaders[21].sh_name));
            Assert.AreEqual(".data", el.GetSectionName(el.SectionHeaders[22].sh_name));
            Assert.AreEqual(".bss", el.GetSectionName(el.SectionHeaders[23].sh_name));
            Assert.AreEqual(".comment", el.GetSectionName(el.SectionHeaders[24].sh_name));
            Assert.AreEqual(".shstrtab", el.GetSectionName(el.SectionHeaders[25].sh_name));
            Assert.AreEqual(".symtab", el.GetSectionName(el.SectionHeaders[26].sh_name));
            Assert.AreEqual(".strtab", el.GetSectionName(el.SectionHeaders[27].sh_name));
        }

        [Test]
        public void EIL_LoadProgramHeaders()
        {
            mr.ReplayAll();

            var el = new ElfImageLoader(services, "foo", rawImg);
            el.Load(new Address(0));
            el.Dump(Console.Out);
        }
    }
}
