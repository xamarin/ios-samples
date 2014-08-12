.subsections_via_symbols
.section __DWARF, __debug_abbrev,regular,debug

	.byte 1,17,1,37,8,3,8,27,8,19,11,17,1,18,1,16,6,0,0,2,46,1,3,8,17,1,18,1,64,10,0,0
	.byte 3,5,0,3,8,73,19,2,10,0,0,15,5,0,3,8,73,19,2,6,0,0,4,36,0,11,11,62,11,3,8,0
	.byte 0,5,2,1,3,8,11,15,0,0,17,2,0,3,8,11,15,0,0,6,13,0,3,8,73,19,56,10,0,0,7,22
	.byte 0,3,8,73,19,0,0,8,4,1,3,8,11,15,73,19,0,0,9,40,0,3,8,28,13,0,0,10,57,1,3,8
	.byte 0,0,11,52,0,3,8,73,19,2,10,0,0,12,52,0,3,8,73,19,2,6,0,0,13,15,0,73,19,0,0,14
	.byte 16,0,73,19,0,0,16,28,0,73,19,56,10,0,0,18,46,0,3,8,17,1,18,1,0,0,0
.section __DWARF, __debug_info,regular,debug
Ldebug_info_start:

LDIFF_SYM0=Ldebug_info_end - Ldebug_info_begin
	.long LDIFF_SYM0
Ldebug_info_begin:

	.short 2
	.long 0
	.byte 4,1
	.asciz "Mono AOT Compiler 3.6.0 (mono-3.6.0-branch/0d48422 Mon Aug 11 15:22:27 EDT 2014)"
	.asciz "System.Core.dll"
	.asciz ""

	.byte 2,0,0,0,0,0,0,0,0
LDIFF_SYM1=Ldebug_line_start - Ldebug_line_section_start
	.long LDIFF_SYM1
LDIE_I1:

	.byte 4,1,5
	.asciz "sbyte"
LDIE_U1:

	.byte 4,1,7
	.asciz "byte"
LDIE_I2:

	.byte 4,2,5
	.asciz "short"
LDIE_U2:

	.byte 4,2,7
	.asciz "ushort"
LDIE_I4:

	.byte 4,4,5
	.asciz "int"
LDIE_U4:

	.byte 4,4,7
	.asciz "uint"
LDIE_I8:

	.byte 4,8,5
	.asciz "long"
LDIE_U8:

	.byte 4,8,7
	.asciz "ulong"
LDIE_I:

	.byte 4,4,5
	.asciz "intptr"
LDIE_U:

	.byte 4,4,7
	.asciz "uintptr"
LDIE_R4:

	.byte 4,4,4
	.asciz "float"
LDIE_R8:

	.byte 4,8,4
	.asciz "double"
LDIE_BOOLEAN:

	.byte 4,1,2
	.asciz "boolean"
LDIE_CHAR:

	.byte 4,2,8
	.asciz "char"
LDIE_STRING:

	.byte 4,4,1
	.asciz "string"
LDIE_OBJECT:

	.byte 4,4,1
	.asciz "object"
LDIE_SZARRAY:

	.byte 4,4,1
	.asciz "object"
.section __DWARF, __debug_loc,regular,debug
Ldebug_loc_start:
.section __DWARF, __debug_frame,regular,debug
	.align 3

LDIFF_SYM2=Lcie0_end - Lcie0_start
	.long LDIFF_SYM2
Lcie0_start:

	.long -1
	.byte 3
	.asciz ""

	.byte 1,124,14
	.align 2
Lcie0_end:
.text
	.align 3
methods:
	.space 16
.text
	.align 2
	.no_dead_strip _System_Linq_Check_Source_object
_System_Linq_Check_Source_object:

	.byte 128,64,45,233,13,112,160,225,0,1,45,233,12,208,77,226,0,0,141,229,0,0,157,229,0,0,80,227,2,0,0,10
	.byte 12,208,141,226,0,1,189,232,128,128,189,232,0,0,159,229,0,0,0,234
	.long _mono_aot_System_Core_got - . -12
	.byte 0,0,159,231,1,16,160,227
bl _p_1

	.byte 0,16,160,225,123,1,0,227,0,2,64,227
bl _mono_create_corlib_exception_1
bl _p_2

Lme_0:
.text
	.align 2
	.no_dead_strip _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource
_System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource:

	.byte 128,64,45,233,13,112,160,225,0,1,45,233,20,208,77,226,0,128,141,229,4,0,141,229,4,0,157,229
bl _p_3

	.byte 0,0,157,229
bl _p_4
bl _p_5

	.byte 8,0,141,229,4,16,157,229
bl _p_6

	.byte 8,0,157,229,20,208,141,226,0,1,189,232,128,128,189,232

Lme_1:
.text
	.align 2
	.no_dead_strip _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0
_System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0:

	.byte 128,64,45,233,13,112,160,225,0,9,45,233,24,208,77,226,13,176,160,225,4,128,139,229,12,0,139,229,4,0,155,229
bl _p_7

	.byte 0,0,139,229,0,0,144,229,0,0,160,227,8,0,139,229,12,0,155,229
bl _p_3

	.byte 4,0,155,229
bl _p_8
bl _p_5

	.byte 20,0,139,229,4,0,155,229
bl _p_9

	.byte 0,32,160,225,20,0,155,229,16,0,139,229,12,16,155,229,50,255,47,225,16,0,155,229,24,208,139,226,0,9,189,232
	.byte 128,128,189,232

Lme_3:
.text
	.align 3
methods_end:

	.long 0
.text
	.align 3
method_addresses:
	.no_dead_strip method_addresses
bl _System_Linq_Check_Source_object
bl _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource
bl method_addresses
bl _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0
method_addresses_end:
.section __TEXT, __const
	.align 3
code_offsets:

	.long 0

.text
	.align 3
unbox_trampolines:
unbox_trampolines_end:

	.long 0
.section __TEXT, __const
	.align 3
method_info_offsets:

	.long 4,10,1,2
	.short 0
	.byte 1,2,255,255,255,255,253,5
.section __TEXT, __const
	.align 3
extra_method_table:

	.long 11,0,0,0,0,0,0,0
	.long 0,0,0,0,0,0,0,0
	.long 12,3,0,0,0,0,0,0
	.long 0,0,0,0,0,0,0,0
	.long 0,0
.section __TEXT, __const
	.align 3
extra_method_info_offsets:

	.long 1,3,12
.section __TEXT, __const
	.align 3
class_name_table:

	.short 11, 1, 0, 0, 0, 0, 0, 0
	.short 0, 0, 0, 2, 0, 0, 0, 3
	.short 0, 0, 0, 0, 0, 0, 0
.section __TEXT, __const
	.align 3
got_info_offsets:

	.long 4,10,1,2
	.short 0
	.byte 28,2,1,1
.section __TEXT, __const
	.align 3
ex_info_offsets:

	.long 4,10,1,2
	.short 0
	.byte 128,255,39,255,255,255,254,218,129,85
.section __TEXT, __const
	.align 3
unwind_info:

	.byte 18,12,13,0,72,14,8,135,2,68,14,12,136,3,142,1,68,14,24,18,12,13,0,72,14,8,135,2,68,14,12,136
	.byte 3,142,1,68,14,32,23,12,13,0,72,14,8,135,2,68,14,16,136,4,139,3,142,1,68,14,40,68,13,11
.section __TEXT, __const
	.align 3
class_info_offsets:

	.long 3,10,1,2
	.short 0
	.byte 129,153,7,23

.text
	.align 4
plt:
_mono_aot_System_Core_plt:
	.no_dead_strip plt__jit_icall_mono_helper_ldstr
plt__jit_icall_mono_helper_ldstr:
_p_1:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 16,33
	.no_dead_strip plt__jit_icall_mono_arch_throw_exception
plt__jit_icall_mono_arch_throw_exception:
_p_2:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 20,53
	.no_dead_strip plt_System_Linq_Check_Source_object
plt_System_Linq_Check_Source_object:
_p_3:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 24,81
	.no_dead_strip plt__rgctx_fetch_0
plt__rgctx_fetch_0:
_p_4:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 28,116
	.no_dead_strip plt__jit_icall_mono_object_new_specific
plt__jit_icall_mono_object_new_specific:
_p_5:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 32,122
	.no_dead_strip plt_System_Collections_Generic_List_1_TSource__ctor_System_Collections_Generic_IEnumerable_1_TSource
plt_System_Collections_Generic_List_1_TSource__ctor_System_Collections_Generic_IEnumerable_1_TSource:
_p_6:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 36,149
	.no_dead_strip plt__rgctx_fetch_1
plt__rgctx_fetch_1:
_p_7:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 40,182
	.no_dead_strip plt__rgctx_fetch_2
plt__rgctx_fetch_2:
_p_8:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 44,213
	.no_dead_strip plt__rgctx_fetch_3
plt__rgctx_fetch_3:
_p_9:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 48,221
plt_end:
.section __TEXT, __const
	.align 3
image_table:

	.long 2
	.asciz "System.Core"
	.asciz "4060FAC8-D8A5-4C8E-AE9B-B327FDE9A303"
	.asciz ""
	.asciz "7cec85d7bea7798e"
	.align 3

	.long 1,2,0,5,0
	.asciz "mscorlib"
	.asciz "C1B3CD7F-E42C-4FF1-A583-0DBA0D791DFB"
	.asciz ""
	.asciz "7cec85d7bea7798e"
	.align 3

	.long 1,2,0,5,0
.data
	.align 3
_mono_aot_System_Core_got:
	.space 56
got_end:
.section __TEXT, __const
	.align 2
assembly_guid:
	.asciz "4060FAC8-D8A5-4C8E-AE9B-B327FDE9A303"
.section __TEXT, __const
	.align 2
runtime_version:
	.asciz ""
.section __TEXT, __const
	.align 2
assembly_name:
	.asciz "System.Core"
.data
	.align 3
_mono_aot_file_info:

	.long 100,0
	.align 2
	.long _mono_aot_System_Core_got
	.align 2
	.long methods
	.align 2
	.long 0
	.align 2
	.long blob
	.align 2
	.long class_name_table
	.align 2
	.long class_info_offsets
	.align 2
	.long method_info_offsets
	.align 2
	.long ex_info_offsets
	.align 2
	.long code_offsets
	.align 2
	.long method_addresses
	.align 2
	.long extra_method_info_offsets
	.align 2
	.long extra_method_table
	.align 2
	.long got_info_offsets
	.align 2
	.long methods_end
	.align 2
	.long unwind_info
	.align 2
	.long mem_end
	.align 2
	.long image_table
	.align 2
	.long plt
	.align 2
	.long plt_end
	.align 2
	.long assembly_guid
	.align 2
	.long runtime_version
	.align 2
	.long 0
	.align 2
	.long 0
	.align 2
	.long 0
	.align 2
	.long 0
	.align 2
	.long 0
	.align 2
	.long globals
	.align 2
	.long assembly_name
	.align 2
	.long unbox_trampolines
	.align 2
	.long unbox_trampolines_end

	.long 4,56,10,4,10,387000831,0,462
	.long 0,0,0,0,0,0,0,0
	.long 0,0,0,0,128,4,4,14
	.long 0,0,0,0,0
	.globl _mono_aot_module_System_Core_info
	.align 2
_mono_aot_module_System_Core_info:
	.align 2
	.long _mono_aot_file_info
.section __TEXT, __const
	.align 3
blob:

	.byte 0,0,0,0,0,0,0,5,30,0,0,1,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,7,12,0,39,42
	.byte 47,7,17,109,111,110,111,95,104,101,108,112,101,114,95,108,100,115,116,114,0,7,25,109,111,110,111,95,97,114,99,104
	.byte 95,116,104,114,111,119,95,101,120,99,101,112,116,105,111,110,0,3,1,5,30,0,1,255,255,255,255,255,2,255,253,0
	.byte 0,0,1,3,0,198,0,0,2,0,1,7,83,4,2,59,1,1,7,83,35,93,150,5,7,109,7,24,109,111,110,111
	.byte 95,111,98,106,101,99,116,95,110,101,119,95,115,112,101,99,105,102,105,99,0,3,255,253,0,0,0,7,109,1,198,0
	.byte 1,10,1,7,83,0,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,7,35,128,166,192,0,92,41,255,253,0
	.byte 0,0,1,3,0,198,0,0,2,0,1,7,7,0,4,2,59,1,1,7,7,35,128,166,150,5,7,128,206,35,128,166
	.byte 192,0,90,33,32,1,1,21,2,51,1,1,7,7,255,253,0,0,0,7,128,206,1,198,0,1,10,1,7,7,0,2
	.byte 0,36,32,24,88,208,0,0,13,0,0,11,1,24,0,4,5,16,0,16,0,4,0,4,5,4,0,8,5,4,0,4
	.byte 2,255,255,255,255,200,3,19,0,1,13,0,16,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,83,0,22,60
	.byte 28,72,208,0,0,13,4,0,6,1,28,6,8,0,4,0,4,0,8,6,8,3,38,0,1,11,4,16,255,253,0,0
	.byte 0,1,3,0,198,0,0,2,0,1,7,7,1,0,1,1,40,108,32,120,208,0,0,11,12,1,208,0,0,11,0,208
	.byte 0,0,11,8,10,0,32,0,8,1,16,6,8,0,4,0,8,0,4,0,4,0,16,6,8,0,128,144,8,0,0,1
	.byte 4,128,144,8,0,0,1,193,0,8,255,193,0,8,252,193,0,8,251,193,0,8,249,4,128,144,8,0,0,1,193,0
	.byte 8,255,193,0,8,252,193,0,8,251,193,0,8,249,98,111,101,104,109,0
.section __TEXT, __const
	.align 3
Lglobals_hash:

	.short 11, 0, 0, 0, 0, 0, 0, 0
	.short 0, 0, 0, 0, 0, 0, 0, 0
	.short 0, 0, 0, 0, 0, 0, 0
.data
	.align 3
globals:
	.align 2
	.long Lglobals_hash

	.long 0,0
.section __DWARF, __debug_info,regular,debug
LTDIE_0:

	.byte 17
	.asciz "System_Object"

	.byte 8,7
	.asciz "System_Object"

LDIFF_SYM3=LTDIE_0 - Ldebug_info_start
	.long LDIFF_SYM3
LTDIE_0_POINTER:

	.byte 13
LDIFF_SYM4=LTDIE_0 - Ldebug_info_start
	.long LDIFF_SYM4
LTDIE_0_REFERENCE:

	.byte 14
LDIFF_SYM5=LTDIE_0 - Ldebug_info_start
	.long LDIFF_SYM5
	.byte 2
	.asciz "System.Linq.Check:Source"
	.long _System_Linq_Check_Source_object
	.long Lme_0

	.byte 2,118,16,3
	.asciz "source"

LDIFF_SYM6=LDIE_OBJECT - Ldebug_info_start
	.long LDIFF_SYM6
	.byte 2,125,0,0

.section __DWARF, __debug_frame,regular,debug

LDIFF_SYM7=Lfde0_end - Lfde0_start
	.long LDIFF_SYM7
Lfde0_start:

	.long 0
	.align 2
	.long _System_Linq_Check_Source_object

LDIFF_SYM8=Lme_0 - _System_Linq_Check_Source_object
	.long LDIFF_SYM8
	.byte 12,13,0,72,14,8,135,2,68,14,12,136,3,142,1,68,14,24
	.align 2
Lfde0_end:

.section __DWARF, __debug_info,regular,debug
LTDIE_1:

	.byte 17
	.asciz "System_Collections_Generic_IEnumerable`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IEnumerable`1"

LDIFF_SYM9=LTDIE_1 - Ldebug_info_start
	.long LDIFF_SYM9
LTDIE_1_POINTER:

	.byte 13
LDIFF_SYM10=LTDIE_1 - Ldebug_info_start
	.long LDIFF_SYM10
LTDIE_1_REFERENCE:

	.byte 14
LDIFF_SYM11=LTDIE_1 - Ldebug_info_start
	.long LDIFF_SYM11
	.byte 2
	.asciz "System.Linq.Enumerable:ToList<TSource>"
	.long _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource
	.long Lme_1

	.byte 2,118,16,3
	.asciz "source"

LDIFF_SYM12=LTDIE_1_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM12
	.byte 2,125,4,0

.section __DWARF, __debug_frame,regular,debug

LDIFF_SYM13=Lfde1_end - Lfde1_start
	.long LDIFF_SYM13
Lfde1_start:

	.long 0
	.align 2
	.long _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource

LDIFF_SYM14=Lme_1 - _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource
	.long LDIFF_SYM14
	.byte 12,13,0,72,14,8,135,2,68,14,12,136,3,142,1,68,14,32
	.align 2
Lfde1_end:

.section __DWARF, __debug_info,regular,debug
LTDIE_2:

	.byte 17
	.asciz "System_Collections_Generic_IEnumerable`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IEnumerable`1"

LDIFF_SYM15=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM15
LTDIE_2_POINTER:

	.byte 13
LDIFF_SYM16=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM16
LTDIE_2_REFERENCE:

	.byte 14
LDIFF_SYM17=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM17
	.byte 2
	.asciz "System.Linq.Enumerable:ToList<!!0>"
	.long _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0
	.long Lme_3

	.byte 2,118,16,3
	.asciz "source"

LDIFF_SYM18=LTDIE_2_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM18
	.byte 2,123,12,0

.section __DWARF, __debug_frame,regular,debug

LDIFF_SYM19=Lfde2_end - Lfde2_start
	.long LDIFF_SYM19
Lfde2_start:

	.long 0
	.align 2
	.long _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0

LDIFF_SYM20=Lme_3 - _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0
	.long LDIFF_SYM20
	.byte 12,13,0,72,14,8,135,2,68,14,16,136,4,139,3,142,1,68,14,40,68,13,11
	.align 2
Lfde2_end:

.section __DWARF, __debug_info,regular,debug

	.byte 0
Ldebug_info_end:
.section __DWARF, __debug_line,regular,debug
Ldebug_line_section_start:
Ldebug_line_start:

	.long Ldebug_line_end - . -4
	.short 2
	.long Ldebug_line_header_end - . -4
	.byte 1,1,251,14,13,0,1,1,1,1,0,0,0,1,0,0,1
.section __DWARF, __debug_line,regular,debug
	.asciz "/Developer/MonoTouch/Source/mono/mcs/class/System.Core/System.Linq"

	.byte 0
	.asciz "<unknown>"

	.byte 0,0,0
	.asciz "Check.cs"

	.byte 1,0,0
	.asciz "Enumerable.cs"

	.byte 1,0,0,0
Ldebug_line_header_end:
.section __DWARF, __debug_line,regular,debug

	.byte 0,5,2
	.long _System_Linq_Check_Source_object

	.byte 3,36,4,2,1,3,36,2,24,1,131,2,56,1,0,1,1
.section __DWARF, __debug_line,regular,debug

	.byte 0,5,2
	.long _System_Linq_Enumerable_ToList_TSource_System_Collections_Generic_IEnumerable_1_TSource

	.byte 3,133,23,4,3,1,3,133,23,2,28,1,132,2,36,1,0,1,1
.section __DWARF, __debug_line,regular,debug

	.byte 0,5,2
	.long _System_Linq_Enumerable_ToList___0_System_Collections_Generic_IEnumerable_1___0

	.byte 3,133,23,4,3,1,3,133,23,2,32,1,8,230,2,56,1,0,1,1,0,1,1
Ldebug_line_end:
.text
	.align 3
mem_end:
