﻿using static Forza_Mods_AIO.Resources.Cheats;
using static Forza_Mods_AIO.Resources.Memory;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon5;

public class UnlocksCheats : CheatsUtilities, ICheatsBase
{
    public UIntPtr CreditsAddress, CreditsDetourAddress;
    public UIntPtr XpPointsAddress, XpPointsDetourAddress;
    public UIntPtr XpAddress, XpDetourAddress;
    public UIntPtr SpinsAddress, SpinsDetourAddress;
    public UIntPtr SkillPointsAddress, SkillPointsDetourAddress;
    public UIntPtr SeriesAddress, SeriesDetourAddress;
    public UIntPtr SeasonalAddress, SeasonalDetourAddress;

    public async Task CheatCredits()
    {
        CreditsAddress = 0;
        CreditsDetourAddress = 0;
        
        const string sig = "E8 ? ? ? ? 89 84 ? ? ? ? ? 4C 8D ? ? ? ? ? 48 8B";
        CreditsAddress = await SmartAobScan(sig);

        if (CreditsAddress > 0)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;
            
            var relativeAddress = CreditsAddress + 1;
            var relativeOffset = GetInstance().ReadMemory<int>(relativeAddress);
            CreditsAddress = (UIntPtr)((IntPtr)CreditsAddress + relativeOffset + 0x5 + 24);
            
            var asm = new byte[]
            {
                0x48, 0x8B, 0x4F, 0x08, 0x80, 0x3D, 0x26, 0x00, 0x00, 0x00, 0x01, 0x75, 0x1D, 0x48, 0x8B, 0x54, 0x24,
                0x20, 0x48, 0xB8, 0x43, 0x72, 0x65, 0x64, 0x69, 0x74, 0x73, 0x00, 0x48, 0x39, 0x42, 0xB4, 0x75, 0x08,
                0x8B, 0x15, 0x0A, 0x00, 0x00, 0x00, 0x89, 0x17, 0x31, 0xD2
            };

            CreditsDetourAddress = GetInstance().CreateDetour(CreditsAddress, asm, 6);
            return;
        }
        
        ShowError("Credits", sig);
    }

    public async Task CheatXp()
    {
        XpPointsAddress = 0;
        XpPointsDetourAddress = 0;
        XpAddress = 0;
        XpDetourAddress = 0;

        const string sig = "44 89 ? ? 8B 89 ? ? ? ? 85 C9";
        XpPointsAddress = await SmartAobScan(sig) + 4;
        if (XpPointsAddress > 4)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;
            
            XpAddress = XpPointsAddress + 14;
            var pointsAsm = new byte[]
            {
                0x80, 0x3D, 0x14, 0x00, 0x00, 0x00, 0x01, 0x75, 0x07, 0xC6, 0x81, 0x88, 0x00, 0x00, 0x00, 0x01, 0x8B,
                0x89, 0x88, 0x00, 0x00, 0x00
            };

            var asm = new byte[]
            {
                0x41, 0x8B, 0x87, 0x8C, 0x00, 0x00, 0x00, 0x80, 0x3D, 0x0D, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B,
                0x05, 0x06, 0x00, 0x00, 0x00
            };

            XpPointsDetourAddress = GetInstance().CreateDetour(XpPointsAddress, pointsAsm, 6);
            XpDetourAddress = GetInstance().CreateDetour(XpAddress, asm, 7);
            return;
        }
        
        ShowError("Xp", sig);
    }

    public async Task CheatSpins()
    {
        SpinsAddress = 0;
        SpinsDetourAddress = 0;

        const string sig = "48 89 5C 24 08 57 48 83 EC 20 48 8B FA 33 D2 48 8B 4F 10";
        SpinsAddress = await SmartAobScan(sig) + 28;

        if (SpinsAddress > 28)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;
            
            var asm = new byte[]
            {
                0x80, 0x3D, 0x15, 0x00, 0x00, 0x00, 0x01, 0x75, 0x09, 0x8B, 0x15, 0x0E, 0x00, 0x00, 0x00, 0x89, 0x57,
                0x08, 0x33, 0xD2, 0x8B, 0x5F, 0x08
            };

            SpinsDetourAddress = GetInstance().CreateDetour(SpinsAddress, asm, 5);
            return;
        }
        
        ShowError("Spins", sig);
    }
    
    public async Task CheatSkillPoints()
    {
        SkillPointsAddress = 0;
        SkillPointsDetourAddress = 0;

        const string sig = "85 D2 78 32 48 89 5C 24 08 57 48 83 EC 20 8B DA 48 8B F9 48 8B 49 48";
        SkillPointsAddress = await SmartAobScan(sig) + 34;

        if (SkillPointsAddress > 34)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;

            var asm = new byte[]
            {
                0x80, 0x3D, 0x12, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B, 0x1D, 0x0B, 0x00, 0x00, 0x00, 0x33, 0xD2,
                0x89, 0x5F, 0x40
            };

            SkillPointsDetourAddress = GetInstance().CreateDetour(SkillPointsAddress, asm, 5);
            return;
        }
        
        ShowError("Skill points", sig);
    }

    public async Task CheatSeasonal()
    {
        SeasonalAddress = 0;
        SeasonalDetourAddress = 0;

        const string sig = "49 63 ? 8B 44 ? ? C3";
        SeasonalAddress = await SmartAobScan(sig);

        if (SeasonalAddress > 0)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;

            var asm = new byte[]
            {
                0x49, 0x63, 0xC0, 0x80, 0x3D, 0x19, 0x00, 0x00, 0x00, 0x01, 0x75, 0x0E, 0x52, 0x48, 0x8B, 0x15, 0x10,
                0x00, 0x00, 0x00, 0x48, 0x89, 0x54, 0x81, 0x60, 0x5A, 0x8B, 0x44, 0x81, 0x60
            };

            SeasonalDetourAddress = GetInstance().CreateDetour(SeasonalAddress, asm, 7);
            return;
        }
        
        ShowError("Seasonal points", sig);
    }

    public async Task CheatSeries()
    {
        SeriesAddress = 0;
        SeriesDetourAddress = 0;

        const string sig = "89 59 ? 48 83 C4 ? 5B C3 CC CC CC CC CC 44 89";
        SeriesAddress = await SmartAobScan(sig);

        if (SeriesAddress > 0)
        {
            if (GetClass<Bypass>().CrcFuncDetourAddress == 0)
            {
                await GetClass<Bypass>().DisableCrcChecks();
            }
            
            if (GetClass<Bypass>().CrcFuncDetourAddress <= 0) return;

            var asm = new byte[]
            {
                0x80, 0x3D, 0x14, 0x00, 0x00, 0x00, 0x01, 0x75, 0x06, 0x8B, 0x1D, 0x0D, 0x00, 0x00, 0x00, 0x89, 0x59,
                0x14, 0x48, 0x83, 0xC4, 0x30
            };

            SeriesDetourAddress = GetInstance().CreateDetour(SeriesAddress, asm, 7);
            return;
        }
        
        ShowError("Series points", sig);
    }
    
    public void Cleanup()
    {
        var mem = GetInstance();
        
        if (CreditsAddress > 0)
        {
            mem.WriteArrayMemory(CreditsAddress, new byte[] { 0x89, 0x84, 0x24, 0x80, 0x00, 0x00, 0x00 });
            Free(CreditsDetourAddress);
        }

        if (XpPointsAddress > 4)
        {
            mem.WriteArrayMemory(XpPointsAddress, new byte[] { 0x8B, 0x89, 0x88, 0x00, 0x00, 0x00 });
            Free(XpPointsDetourAddress);
        }

        if (XpAddress > 0)
        {
            mem.WriteArrayMemory(XpAddress, new byte[] { 0x41, 0x8B, 0x87, 0x8C, 0x00, 0x00, 0x00 });
            Free(XpDetourAddress);
        }

        if (SpinsAddress > 28)
        {
            mem.WriteArrayMemory(SpinsAddress, new byte[] { 0x33, 0xD2, 0x8B, 0x5F, 0x08 });
            Free(SpinsDetourAddress);
        }

        if (SkillPointsAddress > 34)
        {
            mem.WriteArrayMemory(SkillPointsAddress, new byte[] { 0x33, 0xD2, 0x89, 0x5F, 0x40 });
            Free(SkillPointsDetourAddress);
        }

        if (SeasonalAddress > 0)
        {
            mem.WriteArrayMemory(SkillPointsAddress, new byte[] { 0x49, 0x63, 0xC0, 0x8B, 0x44, 0x81, 0x60 });
            Free(SeasonalDetourAddress);
        }

        if (SeriesAddress > 0)
        {
            mem.WriteArrayMemory(SeriesAddress, new byte[] { 0x89, 0x59, 0x14, 0x48, 0x83, 0xC4, 0x30 });
            Free(SeriesDetourAddress);
        }
    }

    public void Reset()
    {
        var fields = typeof(UnlocksCheats).GetFields().Where(f => f.FieldType == typeof(UIntPtr));
        foreach (var field in fields)
        {
            field.SetValue(this, UIntPtr.Zero);
        }
    }
}