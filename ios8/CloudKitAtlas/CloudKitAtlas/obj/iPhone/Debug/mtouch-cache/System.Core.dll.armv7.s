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
	.no_dead_strip _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource
_System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource:

	.byte 128,64,45,233,13,112,160,225,112,13,45,233,40,208,77,226,13,176,160,225,12,128,139,229,0,160,160,225,0,0,160,227
	.byte 8,0,139,229,10,0,160,225
bl _p_3

	.byte 12,0,155,229
bl _p_4

	.byte 0,32,160,225,4,16,146,229,10,0,160,225
bl _p_5

	.byte 0,96,160,225,0,0,80,227,39,0,0,10,12,0,155,229
bl _p_6

	.byte 0,32,160,225,6,0,160,225,0,16,150,229,2,128,160,225,4,224,143,226,76,240,17,229,0,0,0,0,0,0,80,227
	.byte 22,0,0,218,12,0,155,229
bl _p_6

	.byte 0,32,160,225,6,0,160,225,0,16,150,229,2,128,160,225,4,224,143,226,76,240,17,229,0,0,0,0,1,0,64,226
	.byte 32,0,139,229,12,0,155,229
bl _p_7

	.byte 0,48,160,225,32,16,155,229,6,0,160,225,0,32,150,229,3,128,160,225,4,224,143,226,64,240,18,229,0,0,0,0
	.byte 16,0,139,229,3,0,0,234,0,0,160,227,0,0,139,229,0,0,160,227,16,0,139,229,16,0,155,229,68,0,0,234
	.byte 1,0,160,227,4,0,203,229,0,0,160,227,0,0,139,229,0,80,160,227,12,0,155,229
bl _p_8

	.byte 0,32,160,225,10,0,160,225,0,16,154,229,2,128,160,225,4,224,143,226,56,240,17,229,0,0,0,0,8,0,139,229
	.byte 15,0,0,234,8,0,155,229,32,0,139,229,12,0,155,229
bl _p_9

	.byte 0,32,160,225,32,16,155,229,1,0,160,225,0,16,145,229,2,128,160,225,4,224,143,226,16,240,17,229,0,0,0,0
	.byte 0,64,160,225,4,80,160,225,0,0,160,227,4,0,203,229,8,16,155,229,1,0,160,225,0,16,145,229,0,128,159,229
	.byte 0,0,0,234
	.long _mono_aot_System_Core_got - . + 4
	.byte 8,128,159,231,4,224,143,226,60,240,17,229,0,0,0,0,255,0,0,226,0,0,80,227,226,255,255,26,0,0,0,235
	.byte 15,0,0,234,28,224,139,229,8,0,155,229,0,0,80,227,9,0,0,10,8,16,155,229,1,0,160,225,0,16,145,229
	.byte 0,128,159,229,0,0,0,234
	.long _mono_aot_System_Core_got - . + 8
	.byte 8,128,159,231,4,224,143,226,20,240,17,229,0,0,0,0,28,192,155,229,12,240,160,225,4,0,219,229,0,0,80,227
	.byte 1,0,0,26,5,0,160,225,0,0,0,234,5,0,160,225,40,208,139,226,112,13,189,232,128,128,189,232

Lme_1:
.text
	.align 2
	.no_dead_strip _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0
_System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0:

	.byte 128,64,45,233,13,112,160,225,112,13,45,233,40,208,77,226,13,176,160,225,12,128,139,229,0,16,139,229,0,96,160,225
	.byte 12,0,155,229
bl _p_10

	.byte 0,80,160,225,0,0,149,229,7,64,128,226,7,64,196,227,4,208,77,224,0,64,141,226,16,0,149,229,0,0,132,224
	.byte 4,16,149,229,8,32,149,229,50,255,47,225,20,16,149,229,4,0,160,225,1,0,128,224,4,16,149,229,8,32,149,229
	.byte 50,255,47,225,24,16,149,229,4,0,160,225,1,0,128,224,4,16,149,229,8,32,149,229,50,255,47,225,0,0,160,227
	.byte 8,0,139,229,6,0,160,225
bl _p_3

	.byte 12,0,155,229
bl _p_11

	.byte 0,32,160,225,4,16,146,229,6,0,160,225
bl _p_5

	.byte 0,160,160,225,0,0,80,227,54,0,0,10,12,0,155,229
bl _p_12

	.byte 0,32,160,225,10,0,160,225,0,16,154,229,2,128,160,225,4,224,143,226,76,240,17,229,0,0,0,0,0,0,80,227
	.byte 24,0,0,218,12,0,155,229
bl _p_12

	.byte 0,32,160,225,10,0,160,225,0,16,154,229,2,128,160,225,4,224,143,226,76,240,17,229,0,0,0,0,1,0,64,226
	.byte 32,0,139,229,12,0,155,229
bl _p_13

	.byte 36,0,139,229,12,0,155,229
bl _p_14

	.byte 0,48,160,225,32,32,155,229,36,192,155,229,28,0,149,229,0,16,132,224,10,0,160,225,12,128,160,225,51,255,47,225
	.byte 11,0,0,234,16,0,149,229,0,0,132,224,4,16,149,229,8,32,149,229,50,255,47,225,16,0,149,229,0,16,132,224
	.byte 28,0,149,229,0,0,132,224,4,32,149,229,12,48,149,229,51,255,47,225,28,0,149,229,0,16,132,224,0,0,155,229
	.byte 4,32,149,229,12,48,149,229,51,255,47,225,95,0,0,234,1,0,160,227,4,0,203,229,16,0,149,229,0,0,132,224
	.byte 4,16,149,229,8,32,149,229,50,255,47,225,16,0,149,229,0,16,132,224,20,0,149,229,0,0,132,224,4,32,149,229
	.byte 12,48,149,229,51,255,47,225,12,0,155,229
bl _p_15

	.byte 32,0,139,229,12,0,155,229
bl _p_16

	.byte 0,16,160,225,32,32,155,229,6,0,160,225,2,128,160,225,49,255,47,225,8,0,139,229,22,0,0,234,8,0,155,229
	.byte 32,0,139,229,12,0,155,229
bl _p_17

	.byte 36,0,139,229,12,0,155,229
bl _p_18

	.byte 0,32,160,225,32,0,155,229,36,48,155,229,24,16,149,229,1,16,132,224,3,128,160,225,50,255,47,225,24,0,149,229
	.byte 0,16,132,224,20,0,149,229,0,0,132,224,4,32,149,229,12,48,149,229,51,255,47,225,0,0,160,227,4,0,203,229
	.byte 8,16,155,229,1,0,160,225,0,16,145,229,0,128,159,229,0,0,0,234
	.long _mono_aot_System_Core_got - . + 4
	.byte 8,128,159,231,4,224,143,226,60,240,17,229,0,0,0,0,255,0,0,226,0,0,80,227,219,255,255,26,0,0,0,235
	.byte 15,0,0,234,24,224,139,229,8,0,155,229,0,0,80,227,9,0,0,10,8,16,155,229,1,0,160,225,0,16,145,229
	.byte 0,128,159,229,0,0,0,234
	.long _mono_aot_System_Core_got - . + 8
	.byte 8,128,159,231,4,224,143,226,20,240,17,229,0,0,0,0,24,192,155,229,12,240,160,225,4,0,219,229,0,0,80,227
	.byte 6,0,0,26,20,0,149,229,0,16,132,224,0,0,155,229,4,32,149,229,12,48,149,229,51,255,47,225,5,0,0,234
	.byte 20,0,149,229,0,16,132,224,0,0,155,229,4,32,149,229,12,48,149,229,51,255,47,225,40,208,139,226,112,13,189,232
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
bl _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource
bl method_addresses
bl _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0
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
	.byte 1,2,255,255,255,255,253,7
.section __TEXT, __const
	.align 3
extra_method_table:

	.long 11,0,0,0,0,0,0,0
	.long 0,0,0,0,0,0,0,0
	.long 0,0,0,16,3,0,0,0
	.long 0,0,0,0,0,0,0,0
	.long 0,0
.section __TEXT, __const
	.align 3
extra_method_info_offsets:

	.long 1,3,16
.section __TEXT, __const
	.align 3
class_name_table:

	.short 11, 1, 0, 0, 0, 0, 0, 0
	.short 0, 0, 0, 2, 0, 0, 0, 3
	.short 0, 0, 0, 0, 0, 0, 0
.section __TEXT, __const
	.align 3
got_info_offsets:

	.long 6,10,1,2
	.short 0
	.byte 32,2,1,1,1,5
.section __TEXT, __const
	.align 3
ex_info_offsets:

	.long 4,10,1,2
	.short 0
	.byte 130,13,39,255,255,255,253,204,131,11
.section __TEXT, __const
	.align 3
unwind_info:

	.byte 18,12,13,0,72,14,8,135,2,68,14,12,136,3,142,1,68,14,24,31,12,13,0,72,14,8,135,2,68,14,32,132
	.byte 8,133,7,134,6,136,5,138,4,139,3,142,1,68,14,72,68,13,11
.section __TEXT, __const
	.align 3
class_info_offsets:

	.long 3,10,1,2
	.short 0
	.byte 131,229,7,23

.text
	.align 4
plt:
_mono_aot_System_Core_plt:
	.no_dead_strip plt__jit_icall_mono_helper_ldstr
plt__jit_icall_mono_helper_ldstr:
_p_1:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 24,47
	.no_dead_strip plt__jit_icall_mono_arch_throw_exception
plt__jit_icall_mono_arch_throw_exception:
_p_2:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 28,67
	.no_dead_strip plt_System_Linq_Check_Source_object
plt_System_Linq_Check_Source_object:
_p_3:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 32,95
	.no_dead_strip plt__rgctx_fetch_0
plt__rgctx_fetch_0:
_p_4:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 36,130
	.no_dead_strip plt_wrapper_castclass_object___isinst_with_cache_object_intptr_intptr
plt_wrapper_castclass_object___isinst_with_cache_object_intptr_intptr:
_p_5:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 40,136
	.no_dead_strip plt__rgctx_fetch_1
plt__rgctx_fetch_1:
_p_6:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 44,151
	.no_dead_strip plt__rgctx_fetch_2
plt__rgctx_fetch_2:
_p_7:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 48,172
	.no_dead_strip plt__rgctx_fetch_3
plt__rgctx_fetch_3:
_p_8:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 52,199
	.no_dead_strip plt__rgctx_fetch_4
plt__rgctx_fetch_4:
_p_9:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 56,227
	.no_dead_strip plt__rgctx_fetch_5
plt__rgctx_fetch_5:
_p_10:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 60,264
	.no_dead_strip plt__rgctx_fetch_6
plt__rgctx_fetch_6:
_p_11:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 64,316
	.no_dead_strip plt__rgctx_fetch_7
plt__rgctx_fetch_7:
_p_12:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 68,331
	.no_dead_strip plt__rgctx_fetch_8
plt__rgctx_fetch_8:
_p_13:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 72,353
	.no_dead_strip plt__rgctx_fetch_9
plt__rgctx_fetch_9:
_p_14:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 76,375
	.no_dead_strip plt__rgctx_fetch_10
plt__rgctx_fetch_10:
_p_15:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 80,412
	.no_dead_strip plt__rgctx_fetch_11
plt__rgctx_fetch_11:
_p_16:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 84,434
	.no_dead_strip plt__rgctx_fetch_12
plt__rgctx_fetch_12:
_p_17:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 88,474
	.no_dead_strip plt__rgctx_fetch_13
plt__rgctx_fetch_13:
_p_18:

	.byte 0,192,159,229,12,240,159,231
	.long _mono_aot_System_Core_got - . + 92,496
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
	.space 100
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

	.long 6,100,19,4,10,387000831,0,1050
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

	.byte 0,0,0,0,2,4,5,0,2,4,5,5,30,0,0,1,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,11
	.byte 12,0,39,42,47,6,193,0,1,66,6,193,0,8,0,7,17,109,111,110,111,95,104,101,108,112,101,114,95,108,100,115
	.byte 116,114,0,7,25,109,111,110,111,95,97,114,99,104,95,116,104,114,111,119,95,101,120,99,101,112,116,105,111,110,0,3
	.byte 1,5,30,0,1,255,255,255,255,255,2,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,97,4,2,54,1,1
	.byte 7,97,35,107,150,25,7,123,3,255,252,0,0,0,19,10,4,2,48,1,1,7,97,35,107,140,11,255,253,0,0,0
	.byte 7,128,144,1,198,0,0,243,1,7,97,0,35,107,140,11,255,253,0,0,0,7,123,1,198,0,1,3,1,7,97,0
	.byte 4,2,51,1,1,7,97,35,107,140,11,255,253,0,0,0,7,128,192,1,198,0,0,252,1,7,97,0,4,2,52,1
	.byte 1,7,97,35,107,140,11,255,253,0,0,0,7,128,220,1,198,0,0,253,1,7,97,0,255,253,0,0,0,1,3,0
	.byte 198,0,0,2,0,1,7,11,35,128,248,192,0,92,41,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,11,7
	.byte 14,7,11,23,7,11,22,7,11,21,7,11,21,7,11,21,7,11,21,7,11,4,2,54,1,1,7,11,35,128,248,150
	.byte 25,7,129,53,4,2,48,1,1,7,11,35,128,248,140,11,255,253,0,0,0,7,129,68,1,198,0,0,243,1,7,11
	.byte 0,35,128,248,140,11,255,253,0,0,0,7,129,53,1,198,0,1,3,1,7,11,0,35,128,248,192,0,90,35,32,1
	.byte 30,7,11,8,255,253,0,0,0,7,129,53,1,198,0,1,3,1,7,11,0,4,2,51,1,1,7,11,35,128,248,140
	.byte 11,255,253,0,0,0,7,129,149,1,198,0,0,252,1,7,11,0,35,128,248,192,0,90,35,32,0,21,2,52,1,1
	.byte 7,11,255,253,0,0,0,7,129,149,1,198,0,0,252,1,7,11,0,4,2,52,1,1,7,11,35,128,248,140,11,255
	.byte 253,0,0,0,7,129,211,1,198,0,0,253,1,7,11,0,35,128,248,192,0,90,35,32,0,30,7,11,255,253,0,0
	.byte 0,7,129,211,1,198,0,0,253,1,7,11,0,2,0,36,32,24,88,208,0,0,13,0,0,11,1,24,0,4,5,16
	.byte 0,16,0,4,0,4,5,4,0,8,5,4,0,4,2,255,255,255,255,200,7,19,1,2,28,129,236,129,44,129,168,129
	.byte 172,0,1,11,12,16,255,253,0,0,0,1,3,0,198,0,0,2,0,1,7,97,0,128,178,130,4,36,130,16,10,6
	.byte 208,0,0,11,0,208,0,0,11,4,5,4,208,0,0,11,8,0,76,1,36,0,4,6,8,0,4,0,4,0,4,0
	.byte 4,0,4,7,4,0,4,6,8,0,4,0,4,0,4,0,4,0,4,6,12,0,4,7,8,0,4,0,4,0,4,0
	.byte 4,0,4,6,12,1,12,0,4,0,8,0,4,0,4,0,4,5,16,5,8,8,8,1,8,1,8,2,8,9,4,2
	.byte 8,0,4,0,4,0,4,0,4,0,4,7,16,7,16,0,4,0,8,0,4,0,4,0,4,0,12,7,4,3,8,4
	.byte 8,0,4,0,4,0,16,0,12,5,4,0,4,5,4,0,4,7,12,0,4,7,8,0,4,0,4,0,16,5,12,2
	.byte 12,0,4,6,4,0,4,2,4,1,4,7,19,1,2,24,130,212,129,248,130,144,130,148,0,1,11,12,16,255,253,0
	.byte 0,0,1,3,0,198,0,0,2,0,1,7,11,1,0,1,1,128,178,131,20,36,131,32,6,10,255,80,0,0,3,208
	.byte 0,0,11,4,255,80,0,0,4,255,80,0,0,5,208,0,0,11,8,1,5,4,71,0,36,0,4,0,4,0,4,1
	.byte 92,0,4,6,8,0,4,0,4,0,4,0,4,0,4,7,4,0,4,6,8,0,4,0,4,0,4,0,4,0,4,6
	.byte 12,0,4,7,8,0,4,0,4,0,4,0,4,0,4,6,12,1,12,0,12,0,4,0,20,0,4,0,4,5,4,14
	.byte 76,1,8,13,56,0,12,0,4,0,8,0,4,0,4,7,8,7,16,0,12,0,4,0,20,0,4,10,36,4,8,0
	.byte 4,0,4,0,16,0,12,5,4,0,4,5,4,0,4,7,12,0,4,7,8,0,4,0,4,0,16,5,12,2,12,0
	.byte 4,6,28,3,28,0,128,144,8,0,0,1,4,128,144,8,0,0,1,193,0,9,29,193,0,9,26,193,0,9,25,193
	.byte 0,9,23,4,128,144,8,0,0,1,193,0,9,29,193,0,9,26,193,0,9,25,193,0,9,23,98,111,101,104,109,0
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
LTDIE_2:

	.byte 17
	.asciz "System_Collections_Generic_IList`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IList`1"

LDIFF_SYM12=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM12
LTDIE_2_POINTER:

	.byte 13
LDIFF_SYM13=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM13
LTDIE_2_REFERENCE:

	.byte 14
LDIFF_SYM14=LTDIE_2 - Ldebug_info_start
	.long LDIFF_SYM14
LTDIE_4:

	.byte 5
	.asciz "System_ValueType"

	.byte 8,16
LDIFF_SYM15=LTDIE_0 - Ldebug_info_start
	.long LDIFF_SYM15
	.byte 2,35,0,0,7
	.asciz "System_ValueType"

LDIFF_SYM16=LTDIE_4 - Ldebug_info_start
	.long LDIFF_SYM16
LTDIE_4_POINTER:

	.byte 13
LDIFF_SYM17=LTDIE_4 - Ldebug_info_start
	.long LDIFF_SYM17
LTDIE_4_REFERENCE:

	.byte 14
LDIFF_SYM18=LTDIE_4 - Ldebug_info_start
	.long LDIFF_SYM18
LTDIE_3:

	.byte 5
	.asciz "System_Boolean"

	.byte 9,16
LDIFF_SYM19=LTDIE_4 - Ldebug_info_start
	.long LDIFF_SYM19
	.byte 2,35,0,6
	.asciz "m_value"

LDIFF_SYM20=LDIE_BOOLEAN - Ldebug_info_start
	.long LDIFF_SYM20
	.byte 2,35,8,0,7
	.asciz "System_Boolean"

LDIFF_SYM21=LTDIE_3 - Ldebug_info_start
	.long LDIFF_SYM21
LTDIE_3_POINTER:

	.byte 13
LDIFF_SYM22=LTDIE_3 - Ldebug_info_start
	.long LDIFF_SYM22
LTDIE_3_REFERENCE:

	.byte 14
LDIFF_SYM23=LTDIE_3 - Ldebug_info_start
	.long LDIFF_SYM23
LTDIE_5:

	.byte 17
	.asciz "System_Collections_Generic_IEnumerator`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IEnumerator`1"

LDIFF_SYM24=LTDIE_5 - Ldebug_info_start
	.long LDIFF_SYM24
LTDIE_5_POINTER:

	.byte 13
LDIFF_SYM25=LTDIE_5 - Ldebug_info_start
	.long LDIFF_SYM25
LTDIE_5_REFERENCE:

	.byte 14
LDIFF_SYM26=LTDIE_5 - Ldebug_info_start
	.long LDIFF_SYM26
	.byte 2
	.asciz "System.Linq.Enumerable:LastOrDefault<TSource>"
	.long _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource
	.long Lme_1

	.byte 2,118,16,3
	.asciz "source"

LDIFF_SYM27=LTDIE_1_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM27
	.byte 1,90,11
	.asciz "list"

LDIFF_SYM28=LTDIE_2_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM28
	.byte 1,86,11
	.asciz ""

LDIFF_SYM29=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM29
	.byte 2,123,0,11
	.asciz "empty"

LDIFF_SYM30=LDIE_BOOLEAN - Ldebug_info_start
	.long LDIFF_SYM30
	.byte 2,123,4,11
	.asciz "item"

LDIFF_SYM31=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM31
	.byte 1,85,11
	.asciz "element"

LDIFF_SYM32=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM32
	.byte 1,84,11
	.asciz ""

LDIFF_SYM33=LTDIE_5_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM33
	.byte 2,123,8,0

.section __DWARF, __debug_frame,regular,debug

LDIFF_SYM34=Lfde1_end - Lfde1_start
	.long LDIFF_SYM34
Lfde1_start:

	.long 0
	.align 2
	.long _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource

LDIFF_SYM35=Lme_1 - _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource
	.long LDIFF_SYM35
	.byte 12,13,0,72,14,8,135,2,68,14,32,132,8,133,7,134,6,136,5,138,4,139,3,142,1,68,14,72,68,13,11
	.align 2
Lfde1_end:

.section __DWARF, __debug_info,regular,debug
LTDIE_6:

	.byte 17
	.asciz "System_Collections_Generic_IEnumerable`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IEnumerable`1"

LDIFF_SYM36=LTDIE_6 - Ldebug_info_start
	.long LDIFF_SYM36
LTDIE_6_POINTER:

	.byte 13
LDIFF_SYM37=LTDIE_6 - Ldebug_info_start
	.long LDIFF_SYM37
LTDIE_6_REFERENCE:

	.byte 14
LDIFF_SYM38=LTDIE_6 - Ldebug_info_start
	.long LDIFF_SYM38
LTDIE_7:

	.byte 17
	.asciz "System_Collections_Generic_IList`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IList`1"

LDIFF_SYM39=LTDIE_7 - Ldebug_info_start
	.long LDIFF_SYM39
LTDIE_7_POINTER:

	.byte 13
LDIFF_SYM40=LTDIE_7 - Ldebug_info_start
	.long LDIFF_SYM40
LTDIE_7_REFERENCE:

	.byte 14
LDIFF_SYM41=LTDIE_7 - Ldebug_info_start
	.long LDIFF_SYM41
LTDIE_8:

	.byte 17
	.asciz "System_Collections_Generic_IEnumerator`1"

	.byte 8,7
	.asciz "System_Collections_Generic_IEnumerator`1"

LDIFF_SYM42=LTDIE_8 - Ldebug_info_start
	.long LDIFF_SYM42
LTDIE_8_POINTER:

	.byte 13
LDIFF_SYM43=LTDIE_8 - Ldebug_info_start
	.long LDIFF_SYM43
LTDIE_8_REFERENCE:

	.byte 14
LDIFF_SYM44=LTDIE_8 - Ldebug_info_start
	.long LDIFF_SYM44
	.byte 2
	.asciz "System.Linq.Enumerable:LastOrDefault<!!0>"
	.long _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0
	.long Lme_3

	.byte 2,118,16,3
	.asciz "source"

LDIFF_SYM45=LTDIE_6_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM45
	.byte 1,86,11
	.asciz "list"

LDIFF_SYM46=LTDIE_7_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM46
	.byte 1,90,11
	.asciz ""

LDIFF_SYM47=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM47
	.byte 1,80,11
	.asciz "empty"

LDIFF_SYM48=LDIE_BOOLEAN - Ldebug_info_start
	.long LDIFF_SYM48
	.byte 2,123,4,11
	.asciz "item"

LDIFF_SYM49=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM49
	.byte 1,80,11
	.asciz "element"

LDIFF_SYM50=LDIE_I4 - Ldebug_info_start
	.long LDIFF_SYM50
	.byte 1,80,11
	.asciz ""

LDIFF_SYM51=LTDIE_8_REFERENCE - Ldebug_info_start
	.long LDIFF_SYM51
	.byte 2,123,8,0

.section __DWARF, __debug_frame,regular,debug

LDIFF_SYM52=Lfde2_end - Lfde2_start
	.long LDIFF_SYM52
Lfde2_start:

	.long 0
	.align 2
	.long _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0

LDIFF_SYM53=Lme_3 - _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0
	.long LDIFF_SYM53
	.byte 12,13,0,72,14,8,135,2,68,14,32,132,8,133,7,134,6,136,5,138,4,139,3,142,1,68,14,72,68,13,11
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
	.long _System_Linq_Enumerable_LastOrDefault_TSource_System_Collections_Generic_IEnumerable_1_TSource

	.byte 3,228,9,4,3,1,3,228,9,2,36,1,188,8,117,187,3,5,2,160,1,1,131,188,3,1,2,220,0,1,131,3
	.byte 3,2,132,1,1,131,132,2,16,1,0,1,1
.section __DWARF, __debug_line,regular,debug

	.byte 0,5,2
	.long _System_Linq_Enumerable_LastOrDefault___0_System_Collections_Generic_IEnumerable_1___0

	.byte 3,228,9,4,3,1,3,228,9,2,36,1,3,2,2,244,0,1,8,117,187,3,5,2,220,1,1,3,3,2,56,1
	.byte 3,2,2,132,1,1,3,3,2,132,1,1,8,229,8,174,2,12,1,0,1,1,0,1,1
Ldebug_line_end:
.text
	.align 3
mem_end:
