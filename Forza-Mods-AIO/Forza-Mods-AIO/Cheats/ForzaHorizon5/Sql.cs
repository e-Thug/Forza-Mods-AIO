﻿using Forza_Mods_AIO.Resources;
using Memory.Types;

namespace Forza_Mods_AIO.Cheats.ForzaHorizon5;

public class Sql : CheatsUtilities
{
    private UIntPtr _cDatabaseAddress;
    private UIntPtr _ptr;
    
    private async Task SqlExecAobScan()
    {
        _cDatabaseAddress = 0;

        const string sig = "0F 84 ? ? ? ? 48 8B 35 ? ? ? ? 48 85 F6 74";
        _cDatabaseAddress = await SmartAobScan(sig);

        if (_cDatabaseAddress > 9)
        {
            var relativeAddress = _cDatabaseAddress + 0x6 + 0x3;
            var relative = Resources.Memory.GetInstance().ReadMemory<int>(relativeAddress);
            var pCDataBaseAddress = _cDatabaseAddress + (nuint)relative + 0x6 + 0x7;
            _ptr = Resources.Memory.GetInstance().ReadMemory<nuint>(pCDataBaseAddress);
            return;
        }

        ShowError("Sql", sig);
    }
    
    private static nuint GetVirtualFunctionPtr(nuint ptr, int index)
    {
        var pVtableBytes = new byte[8];
        var procHandle = Resources.Memory.GetInstance().MProc.Handle;
        Imps.ReadProcessMemory(procHandle, ptr, pVtableBytes, (nuint)pVtableBytes.Length, nint.Zero);

        var pVtable = (nuint)BitConverter.ToInt64(pVtableBytes, 0);
        var vTableBytes = new byte[8];
        var lpBaseAddress = pVtable + (nuint)nuint.Size * (nuint)index;
        Imps.ReadProcessMemory(procHandle, lpBaseAddress, vTableBytes, (nuint)vTableBytes.Length, nint.Zero);
        return (nuint)BitConverter.ToInt64(vTableBytes, 0);
    }
    
    public async Task Query(string command)
    {
        if (_cDatabaseAddress <= 9)
        {
            await SqlExecAobScan();
        }

        if (_cDatabaseAddress <= 9) return;
        var procHandle = Resources.Memory.GetInstance().MProc.Handle;
        var allocShellCodeAddress = Imps.VirtualAllocEx(procHandle, nuint.Zero, 0x1000, 0x3000, 0x40);

        var rcx = _ptr;
        var rdx = Imps.VirtualAllocEx(procHandle, nuint.Zero, 0x1000, 0x3000, 0x40);
        var r8 = Imps.VirtualAllocEx(procHandle, nuint.Zero, 0x1000, 0x3000, 0x40);
        var rdxBytes = BitConverter.GetBytes(rdx.ToUInt64());
        var r8Bytes = BitConverter.GetBytes(rdx.ToUInt64());
        
        const int virtualFunctionIndex = 9;
        var callFunction = GetVirtualFunctionPtr(_ptr, virtualFunctionIndex);
        var callBytes = BitConverter.GetBytes(callFunction.ToUInt64());
        
        byte[] shellCode =
        [
            0x48, 0xBA, rdxBytes[0], rdxBytes[1], rdxBytes[2], rdxBytes[3], rdxBytes[4], rdxBytes[5], rdxBytes[6],
            rdxBytes[7], 0x49, 0xB8, r8Bytes[0], r8Bytes[1], r8Bytes[2], r8Bytes[3], r8Bytes[4], r8Bytes[5], r8Bytes[6],
            r8Bytes[7], 0xFF, 0x25, 0x00, 0x00, 0x00, 0x00, callBytes[0], callBytes[1], callBytes[2], callBytes[3],
            callBytes[4], callBytes[5], callBytes[6], callBytes[7]
        ];
      
        Resources.Memory.GetInstance().WriteStringMemory(r8, command + "\0");

        Imps.WriteProcessMemory(procHandle, allocShellCodeAddress, shellCode, (nuint)shellCode.Length, nint.Zero);
        var handle = Imports.CreateRemoteThread(procHandle, (nint)null, 0, allocShellCodeAddress, rcx, 0, out _);

        _ = Imports.WaitForSingleObject(handle, int.MaxValue);
        Imps.VirtualFreeEx(procHandle, allocShellCodeAddress, 0, Imps.MemRelease);
        Imps.VirtualFreeEx(procHandle, r8, 0, Imps.MemRelease);
    }
}