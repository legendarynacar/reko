// retpoline_stripped.h
// Generated by decompiling retpoline_stripped.elf
// using Reko decompiler version 0.8.0.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals" (400660 Eq_28 t400660) (400710 Eq_31 t400710) (400780 Eq_32 t400780) (600FF8 word64 qw600FF8))
	globals_t (in globals : (ptr64 (struct "Globals")))
Eq_17: (fn void ())
	T_17 (in rdx : (ptr64 Eq_17))
	T_33 (in rtld_fini : (ptr64 (fn void ())))
Eq_18: (union "Eq_18" (int32 u0) (word64 u1))
	T_18 (in qwArg00 : Eq_18)
	T_29 (in argc : int32)
Eq_19: (fn void (ptr64))
	T_19 (in __align : ptr64)
	T_20 (in signature of __align : void)
Eq_26: (fn int32 ((ptr64 Eq_28), Eq_18, (ptr64 (ptr64 char)), (ptr64 Eq_31), (ptr64 Eq_32), (ptr64 Eq_17), (ptr64 void)))
	T_26 (in __libc_start_main : ptr64)
	T_27 (in signature of __libc_start_main : void)
Eq_28: (fn int32 (int32, (ptr64 (ptr64 char)), (ptr64 (ptr64 char))))
	T_28 (in main : (ptr64 (fn int32 (int32, (ptr64 (ptr64 char)), (ptr64 (ptr64 char))))))
	T_35 (in 0x0000000000400660 : word64)
Eq_31: (fn void ())
	T_31 (in init : (ptr64 (fn void ())))
	T_37 (in 0x0000000000400710 : word64)
Eq_32: (fn void ())
	T_32 (in fini : (ptr64 (fn void ())))
	T_38 (in 0x0000000000400780 : word64)
Eq_40: (fn void ())
	T_40 (in __hlt : ptr64)
	T_41 (in signature of __hlt : void)
Eq_63: (fn (ptr64 void) (Eq_65, Eq_65))
	T_63 (in calloc : ptr64)
	T_64 (in signature of calloc : void)
Eq_65: (union "Eq_65" (int64 u0) (size_t u1))
	T_65 (in num : size_t)
	T_66 (in size : size_t)
	T_67 (in (int64) edi : int64)
	T_68 (in (int64) esi : int64)
Eq_70: (fn void (word64))
	T_70 (in fn0000000000400700 : ptr64)
	T_71 (in signature of fn0000000000400700 : void)
Eq_75: (fn void ())
	T_75 (in __pause : ptr64)
	T_76 (in signature of __pause : void)
// Type Variables ////////////
globals_t: (in globals : (ptr64 (struct "Globals")))
  Class: Eq_1
  DataType: (ptr64 Eq_1)
  OrigDataType: (ptr64 (struct "Globals"))
T_2: (in rax_4 : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_3: (in 0000000000600FF8 : ptr64)
  Class: Eq_3
  DataType: (ptr64 word64)
  OrigDataType: (ptr64 (struct (0 T_6 t0000)))
T_4: (in 0x0000000000000000 : word64)
  Class: Eq_4
  DataType: word64
  OrigDataType: word64
T_5: (in 0x0000000000600FF8 + 0x0000000000000000 : word64)
  Class: Eq_5
  DataType: ptr64
  OrigDataType: ptr64
T_6: (in Mem0[0x0000000000600FF8 + 0x0000000000000000:word64] : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_7: (in 0x0000000000000000 : word64)
  Class: Eq_2
  DataType: word64
  OrigDataType: word64
T_8: (in rax_4 == 0x0000000000000000 : bool)
  Class: Eq_8
  DataType: bool
  OrigDataType: bool
T_9: (in rsp_15 : word64)
  Class: Eq_9
  DataType: word64
  OrigDataType: word64
T_10: (in SCZO_16 : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_11: (in rax_17 : word64)
  Class: Eq_11
  DataType: word64
  OrigDataType: word64
T_12: (in SZO_18 : byte)
  Class: Eq_12
  DataType: byte
  OrigDataType: byte
T_13: (in C_19 : bool)
  Class: Eq_13
  DataType: bool
  OrigDataType: bool
T_14: (in Z_20 : bool)
  Class: Eq_14
  DataType: bool
  OrigDataType: bool
T_15: (in __gmon_start__ : ptr64)
  Class: Eq_15
  DataType: (ptr64 code)
  OrigDataType: (ptr64 code)
T_16: (in signature of __gmon_start__ : void)
  Class: Eq_16
  DataType: Eq_16
  OrigDataType: 
T_17: (in rdx : (ptr64 Eq_17))
  Class: Eq_17
  DataType: (ptr64 Eq_17)
  OrigDataType: (ptr64 (fn void ()))
T_18: (in qwArg00 : Eq_18)
  Class: Eq_18
  DataType: Eq_18
  OrigDataType: (union (int32 u1) (word64 u0))
T_19: (in __align : ptr64)
  Class: Eq_19
  DataType: (ptr64 Eq_19)
  OrigDataType: (ptr64 (fn T_25 (T_24)))
T_20: (in signature of __align : void)
  Class: Eq_19
  DataType: (ptr64 Eq_19)
  OrigDataType: 
T_21: (in  : word64)
  Class: Eq_21
  DataType: ptr64
  OrigDataType: 
T_22: (in fp : ptr64)
  Class: Eq_22
  DataType: (ptr64 void)
  OrigDataType: (ptr64 void)
T_23: (in 0x0000000000000008 : word64)
  Class: Eq_23
  DataType: int64
  OrigDataType: int64
T_24: (in fp + 0x0000000000000008 : word64)
  Class: Eq_21
  DataType: ptr64
  OrigDataType: ptr64
T_25: (in __align((char *) fp + 8) : void)
  Class: Eq_25
  DataType: void
  OrigDataType: void
T_26: (in __libc_start_main : ptr64)
  Class: Eq_26
  DataType: (ptr64 Eq_26)
  OrigDataType: (ptr64 (fn T_39 (T_35, T_18, T_36, T_37, T_38, T_17, T_22)))
T_27: (in signature of __libc_start_main : void)
  Class: Eq_26
  DataType: (ptr64 Eq_26)
  OrigDataType: 
T_28: (in main : (ptr64 (fn int32 (int32, (ptr64 (ptr64 char)), (ptr64 (ptr64 char))))))
  Class: Eq_28
  DataType: (ptr64 Eq_28)
  OrigDataType: 
T_29: (in argc : int32)
  Class: Eq_18
  DataType: Eq_18
  OrigDataType: 
T_30: (in ubp_av : (ptr64 (ptr64 char)))
  Class: Eq_30
  DataType: (ptr64 (ptr64 char))
  OrigDataType: 
T_31: (in init : (ptr64 (fn void ())))
  Class: Eq_31
  DataType: (ptr64 Eq_31)
  OrigDataType: 
T_32: (in fini : (ptr64 (fn void ())))
  Class: Eq_32
  DataType: (ptr64 Eq_32)
  OrigDataType: 
T_33: (in rtld_fini : (ptr64 (fn void ())))
  Class: Eq_17
  DataType: (ptr64 Eq_17)
  OrigDataType: 
T_34: (in stack_end : (ptr64 void))
  Class: Eq_22
  DataType: (ptr64 void)
  OrigDataType: 
T_35: (in 0x0000000000400660 : word64)
  Class: Eq_28
  DataType: (ptr64 Eq_28)
  OrigDataType: (ptr64 (fn int32 (int32, (ptr64 (ptr64 char)), (ptr64 (ptr64 char)))))
T_36: (in fp + 0x0000000000000008 : word64)
  Class: Eq_30
  DataType: (ptr64 (ptr64 char))
  OrigDataType: (ptr64 (ptr64 char))
T_37: (in 0x0000000000400710 : word64)
  Class: Eq_31
  DataType: (ptr64 Eq_31)
  OrigDataType: (ptr64 (fn void ()))
T_38: (in 0x0000000000400780 : word64)
  Class: Eq_32
  DataType: (ptr64 Eq_32)
  OrigDataType: (ptr64 (fn void ()))
T_39: (in __libc_start_main(&globals->t400660, qwArg00, (char *) fp + 8, &globals->t400710, &globals->t400780, rdx, fp) : int32)
  Class: Eq_39
  DataType: int32
  OrigDataType: int32
T_40: (in __hlt : ptr64)
  Class: Eq_40
  DataType: (ptr64 Eq_40)
  OrigDataType: (ptr64 (fn T_42 ()))
T_41: (in signature of __hlt : void)
  Class: Eq_40
  DataType: (ptr64 Eq_40)
  OrigDataType: 
T_42: (in __hlt() : void)
  Class: Eq_42
  DataType: void
  OrigDataType: void
T_43: (in r8 : word64)
  Class: Eq_43
  DataType: word64
  OrigDataType: word64
T_44: (in 0x0000000000601040 : word64)
  Class: Eq_43
  DataType: word64
  OrigDataType: word64
T_45: (in r8 == 0x0000000000601040 : bool)
  Class: Eq_45
  DataType: bool
  OrigDataType: bool
T_46: (in 0x0000000000000000 : uint64)
  Class: Eq_46
  DataType: uint64
  OrigDataType: uint64
T_47: (in 0x0000000000000000 : word64)
  Class: Eq_46
  DataType: uint64
  OrigDataType: word64
T_48: (in 0x0000000000000000 == 0x0000000000000000 : bool)
  Class: Eq_48
  DataType: bool
  OrigDataType: bool
T_49: (in rsp_39 : word64)
  Class: Eq_49
  DataType: word64
  OrigDataType: word64
T_50: (in rbp_40 : word64)
  Class: Eq_50
  DataType: word64
  OrigDataType: word64
T_51: (in eax_41 : word32)
  Class: Eq_51
  DataType: word32
  OrigDataType: word32
T_52: (in rax_42 : word64)
  Class: Eq_52
  DataType: word64
  OrigDataType: word64
T_53: (in r8_43 : word64)
  Class: Eq_53
  DataType: word64
  OrigDataType: word64
T_54: (in SCZO_44 : byte)
  Class: Eq_54
  DataType: byte
  OrigDataType: byte
T_55: (in Z_45 : bool)
  Class: Eq_55
  DataType: bool
  OrigDataType: bool
T_56: (in SZO_46 : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_57: (in C_47 : bool)
  Class: Eq_57
  DataType: bool
  OrigDataType: bool
T_58: (in edi_48 : word32)
  Class: Eq_58
  DataType: word32
  OrigDataType: word32
T_59: (in rdi_49 : word64)
  Class: Eq_59
  DataType: word64
  OrigDataType: word64
T_60: (in 0x0000000000000000 : uint64)
  Class: Eq_60
  DataType: (ptr64 code)
  OrigDataType: (ptr64 code)
T_61: (in esi : word32)
  Class: Eq_61
  DataType: word32
  OrigDataType: word32
T_62: (in edi : word32)
  Class: Eq_62
  DataType: word32
  OrigDataType: word32
T_63: (in calloc : ptr64)
  Class: Eq_63
  DataType: (ptr64 Eq_63)
  OrigDataType: (ptr64 (fn T_69 (T_67, T_68)))
T_64: (in signature of calloc : void)
  Class: Eq_63
  DataType: (ptr64 Eq_63)
  OrigDataType: 
T_65: (in num : size_t)
  Class: Eq_65
  DataType: Eq_65
  OrigDataType: 
T_66: (in size : size_t)
  Class: Eq_65
  DataType: Eq_65
  OrigDataType: 
T_67: (in (int64) edi : int64)
  Class: Eq_65
  DataType: Eq_65
  OrigDataType: (union (int64 u0) (size_t u1))
T_68: (in (int64) esi : int64)
  Class: Eq_65
  DataType: Eq_65
  OrigDataType: (union (int64 u0) (size_t u1))
T_69: (in calloc((int64) edi, (int64) esi) : (ptr64 void))
  Class: Eq_69
  DataType: (ptr64 void)
  OrigDataType: (ptr64 void)
T_70: (in fn0000000000400700 : ptr64)
  Class: Eq_70
  DataType: (ptr64 Eq_70)
  OrigDataType: (ptr64 (fn T_74 (T_73)))
T_71: (in signature of fn0000000000400700 : void)
  Class: Eq_70
  DataType: (ptr64 Eq_70)
  OrigDataType: 
T_72: (in qwArg00 : word64)
  Class: Eq_72
  DataType: word64
  OrigDataType: word64
T_73: (in qwLoc08 : word64)
  Class: Eq_72
  DataType: word64
  OrigDataType: word64
T_74: (in fn0000000000400700(qwLoc08) : void)
  Class: Eq_74
  DataType: void
  OrigDataType: void
T_75: (in __pause : ptr64)
  Class: Eq_75
  DataType: (ptr64 Eq_75)
  OrigDataType: (ptr64 (fn T_77 ()))
T_76: (in signature of __pause : void)
  Class: Eq_75
  DataType: (ptr64 Eq_75)
  OrigDataType: 
T_77: (in __pause() : void)
  Class: Eq_77
  DataType: void
  OrigDataType: void
*/
typedef struct Globals {
	Eq_28 t400660;	// 400660
	Eq_31 t400710;	// 400710
	Eq_32 t400780;	// 400780
	word64 qw600FF8;	// 600FF8
} Eq_1;

typedef void (Eq_17)();

typedef union Eq_18 {
	int32 u0;
	word64 u1;
} Eq_18;

typedef void (Eq_19)(ptr64);

typedef int32 (Eq_26)( *, Eq_18, char * *,  *,  *,  *, void);

typedef int32 (Eq_28)(int32 rdi, char * * rsi, char * * rdx);

typedef void (Eq_31)();

typedef void (Eq_32)();

typedef void (Eq_40)();

typedef void (Eq_63)(Eq_65, Eq_65);

typedef union Eq_65 {
	int64 u0;
	size_t u1;
} Eq_65;

typedef void (Eq_70)(word64);

typedef void (Eq_75)();

