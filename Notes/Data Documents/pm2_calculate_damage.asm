; Function: !FN_CalculateDamage (U)
800fd790: stwu	sp, -0x0040 (sp)
800fd794: mflr	r0
800fd798: stw	r0, 0x0044 (sp)
800fd79c: stmw	r20, 0x0010 (sp)
800fd7a0: mr	r21, r4 ; Defender-related parameters
800fd7a4: mr	r20, r3 ; Attacker-related parameters
800fd7a8: mr	r22, r5 ; Defend command-related?
800fd7ac: mr	r23, r6 ; Attack command-related
800fd7b0: mr	r24, r7
800fd7b4: mr	r25, r8
800fd7b8: lwz	r0, 0x027C (r4)
800fd7bc: lwz	r30, 0x1C70 (r13)
800fd7c0: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fd7c4: beq-	 ->0x800FD7CC
800fd7c8: ori	r25, r25, 0x1000
800fd7cc: cmplwi	r20, 0
800fd7d0: lbz	r26, 0x006C (r23)
800fd7d4: beq-	 ->0x800FD7FC
800fd7d8: lwz	r0, 0x0074 (r23)
800fd7dc: rlwinm.	r0, r0, 0, 21, 21 (00000400)
800fd7e0: beq-	 ->0x800FD7FC
800fd7e4: mr	r3, r20
800fd7e8: li	r4, 8
800fd7ec: bl	->0x80127814
800fd7f0: cmpwi	r3, 0
800fd7f4: beq-	 ->0x800FD7FC
800fd7f8: li	r26, 1
800fd7fc: ori	r31, r25, 0x001E
800fd800: stw	r31, 0 (r24)
800fd804: lwz	r12, 0x001C (r23)
800fd808: cmplwi	r12, 0
800fd80c: bne-	 ->0x800FD818
800fd810: li	r3, 0
800fd814: b	->0x800FDE18
800fd818: lwz	r0, 0x01AC (r22)
800fd81c: rlwinm.	r0, r0, 0, 2, 2 (20000000)
800fd820: beq-	 ->0x800FD82C
800fd824: li	r3, 0
800fd828: b	->0x800FDE18
800fd82c: cmplwi	r20, 0
800fd830: beq-	 ->0x800FD868
800fd834: lwz	r0, 0x0074 (r23)
800fd838: rlwinm.	r0, r0, 0, 31, 31 (00000001)
800fd83c: beq-	 ->0x800FD868
800fd840: lbz	r0, 0x02E6 (r20) ; All or Nothing
800fd844: cmplwi	r0, 0
800fd848: beq-	 ->0x800FD868
800fd84c: lwz	r0, 0x1CB8 (r30)
800fd850: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fd854: bne-	 ->0x800FD868
800fd858: rlwinm.	r0, r25, 0, 14, 14 (00020000)
800fd85c: bne-	 ->0x800FD868
800fd860: li	r3, 0
800fd864: b	->0x800FDE18
800fd868: cmplwi	r12, 0
800fd86c: lwz	r29, 0x0020 (r23) ; Base attack power
800fd870: beq-	 ->0x800FD890
800fd874: mr	r3, r20
800fd878: mr	r4, r23
800fd87c: mr	r5, r21
800fd880: mr	r6, r22
800fd884: mtctr	r12
800fd888: bctrl	
800fd88c: mr	r29, r3
800fd890: addis	r3, r30, 2
800fd894: lbz	r0, -0x7007 (r3)
800fd898: cmplwi	r0, 1
800fd89c: bne-	 ->0x800FD8B0
800fd8a0: lwz	r0, 0x0074 (r23)
800fd8a4: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fd8a8: beq-	 ->0x800FD8B0
800fd8ac: addi	r29, r29, 3
800fd8b0: lwz	r0, 0x0074 (r23)
800fd8b4: rlwinm.	r3, r0, 0, 31, 31 (00000001)
800fd8b8: beq-	 ->0x800FD960
800fd8bc: lbz	r5, 0x02E6 (r20) ; All or Nothing
800fd8c0: lbz	r4, 0x02E4 (r20) ; Power Plus
800fd8c4: cmplwi	r5, 0
800fd8c8: lbz	r3, 0x02E5 (r20) ; P-Up, D-Down
800fd8cc: add	r29, r29, r4
800fd8d0: add	r29, r29, r3
800fd8d4: beq-	 ->0x800FD8FC
800fd8d8: lwz	r3, 0x1CB8 (r30)
800fd8dc: rlwinm.	r3, r3, 0, 30, 30 (00000002)
800fd8e0: bne-	 ->0x800FD8EC
800fd8e4: rlwinm.	r3, r25, 0, 14, 14 (00020000)
800fd8e8: beq-	 ->0x800FD8F4
800fd8ec: add	r29, r29, r5
800fd8f0: b	->0x800FD8FC
800fd8f4: li	r29, 0
800fd8f8: b	->0x800FDB1C
800fd8fc: lwz	r4, 0x0010 (r20)
800fd900: lha	r5, 0x010C (r20)
800fd904: lbz	r3, 0x000D (r4)
800fd908: cmpw	r5, r3 ; Is in Peril?
800fd90c: bgt-	 ->0x800FD924
800fd910: lbz	r3, 0x02E7 (r20) ; Mega Rush
800fd914: cmplwi	r3, 0
800fd918: beq-	 ->0x800FD924
800fd91c: mulli	r3, r3, 5
800fd920: add	r29, r29, r3
800fd924: lbz	r3, 0x000C (r4)
800fd928: cmpw	r5, r3 ; Is in Danger?
800fd92c: bgt-	 ->0x800FD944
800fd930: lbz	r3, 0x02E8 (r20) ; Power Rush
800fd934: cmplwi	r3, 0
800fd938: beq-	 ->0x800FD944
800fd93c: rlwinm	r3, r3, 1, 23, 30 (000000ff)
800fd940: add	r29, r29, r3
800fd944: lbz	r4, 0x0301 (r20) ; Ice Power
800fd948: cmplwi	r4, 0
800fd94c: beq-	 ->0x800FD960
800fd950: lwz	r3, 0x01AC (r22)
800fd954: rlwinm.	r3, r3, 0, 23, 23 (00000100)
800fd958: beq-	 ->0x800FD960
800fd95c: add	r29, r29, r4
800fd960: rlwinm.	r0, r0, 0, 29, 29 (00000004)
800fd964: beq-	 ->0x800FD9B0
800fd968: mr	r3, r20
800fd96c: li	r4, 16
800fd970: bl	->0x80127814
800fd974: cmpwi	r3, 0
800fd978: beq-	 ->0x800FD9A4
800fd97c: mr	r3, r20
800fd980: addi	r5, sp, 9
800fd984: addi	r6, sp, 8
800fd988: li	r4, 16
800fd98c: bl	->0x80127F34 ; Get Charge amount
800fd990: lbz	r0, 0x0008 (sp)
800fd994: extsb	r0, r0
800fd998: add.	r29, r29, r0
800fd99c: bge-	 ->0x800FD9A4
800fd9a0: li	r29, 0
800fd9a4: lwz	r0, 0x027C (r20)
800fd9a8: ori	r0, r0, 0x0008
800fd9ac: stw	r0, 0x027C (r20)
800fd9b0: lwz	r0, 0x0074 (r23)
800fd9b4: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fd9b8: beq-	 ->0x800FD9F8
800fd9bc: mr	r3, r20
800fd9c0: li	r4, 10
800fd9c4: bl	->0x80127814
800fd9c8: cmpwi	r3, 0
800fd9cc: beq-	 ->0x800FD9F8
800fd9d0: mr	r3, r20
800fd9d4: addi	r5, sp, 9
800fd9d8: addi	r6, sp, 8
800fd9dc: li	r4, 10
800fd9e0: bl	->0x80127F34
800fd9e4: lbz	r0, 0x0008 (sp)
800fd9e8: extsb	r0, r0
800fd9ec: add.	r29, r29, r0
800fd9f0: bge-	 ->0x800FD9F8
800fd9f4: li	r29, 0
800fd9f8: lwz	r0, 0x0074 (r23)
800fd9fc: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fda00: beq-	 ->0x800FDA40
800fda04: mr	r3, r20
800fda08: li	r4, 12
800fda0c: bl	->0x80127814
800fda10: cmpwi	r3, 0
800fda14: beq-	 ->0x800FDA40
800fda18: mr	r3, r20
800fda1c: addi	r5, sp, 9
800fda20: addi	r6, sp, 8
800fda24: li	r4, 12
800fda28: bl	->0x80127F34
800fda2c: lbz	r0, 0x0008 (sp)
800fda30: extsb	r0, r0
800fda34: add.	r29, r29, r0
800fda38: bge-	 ->0x800FDA40
800fda3c: li	r29, 0
800fda40: lwz	r4, 0x0074 (r23)
800fda44: rlwinm.	r3, r4, 0, 31, 31 (00000001)
800fda48: beq-	 ->0x800FDA54
800fda4c: lbz	r0, 0x02E9 (r20) ; P-Down, D-Up
800fda50: sub	r29, r29, r0
800fda54: cmplwi	r3, 0
800fda58: beq-	 ->0x800FDA6C
800fda5c: lbz	r0, 0x02F9 (r20) ; HP Drain
800fda60: cmplwi	r0, 0
800fda64: beq-	 ->0x800FDA6C
800fda68: sub	r29, r29, r0
800fda6c: cmplwi	r3, 0
800fda70: beq-	 ->0x800FDA84
800fda74: lbz	r0, 0x02FA (r20) ; FP Drain
800fda78: cmplwi	r0, 0
800fda7c: beq-	 ->0x800FDA84
800fda80: sub	r29, r29, r0
800fda84: rlwinm.	r0, r4, 0, 30, 30 (00000002)
800fda88: beq-	 ->0x800FDAC8
800fda8c: mr	r3, r20
800fda90: li	r4, 11
800fda94: bl	->0x80127814
800fda98: cmpwi	r3, 0
800fda9c: beq-	 ->0x800FDAC8
800fdaa0: mr	r3, r20
800fdaa4: addi	r5, sp, 9
800fdaa8: addi	r6, sp, 8
800fdaac: li	r4, 11
800fdab0: bl	->0x80127F34
800fdab4: lbz	r0, 0x0008 (sp)
800fdab8: extsb	r0, r0
800fdabc: add.	r29, r29, r0
800fdac0: bge-	 ->0x800FDAC8
800fdac4: li	r29, 0
800fdac8: lwz	r0, 0x0074 (r23)
800fdacc: rlwinm.	r0, r0, 0, 30, 30 (00000002)
800fdad0: beq-	 ->0x800FDB10
800fdad4: mr	r3, r20
800fdad8: li	r4, 13
800fdadc: bl	->0x80127814
800fdae0: cmpwi	r3, 0
800fdae4: beq-	 ->0x800FDB10
800fdae8: mr	r3, r20
800fdaec: addi	r5, sp, 9
800fdaf0: addi	r6, sp, 8
800fdaf4: li	r4, 13
800fdaf8: bl	->0x80127F34
800fdafc: lbz	r0, 0x0008 (sp)
800fdb00: extsb	r0, r0
800fdb04: add.	r29, r29, r0
800fdb08: bge-	 ->0x800FDB10
800fdb0c: li	r29, 0
800fdb10: cmpwi	r29, 0
800fdb14: bge-	 ->0x800FDB1C
800fdb18: li	r29, 0
800fdb1c: lwz	r4, 0x01B4 (r22) ; Base defense power
800fdb20: rlwinm	r0, r26, 0, 24, 31 (000000ff)
800fdb24: lwz	r3, 0x01B8 (r22)
800fdb28: cmpwi	r0, 2
800fdb2c: lbzx	r28, r4, r0
800fdb30: lbzx	r27, r3, r0
800fdb34: extsb	r28, r28
800fdb38: extsb	r27, r27
800fdb3c: beq-	 ->0x800FDB70
800fdb40: bge-	 ->0x800FDB54
800fdb44: cmpwi	r0, 0
800fdb48: beq-	 ->0x800FDB94
800fdb4c: bge-	 ->0x800FDB64
800fdb50: b	->0x800FDB94
800fdb54: cmpwi	r0, 4
800fdb58: beq-	 ->0x800FDB7C
800fdb5c: bge-	 ->0x800FDB94
800fdb60: b	->0x800FDB88
800fdb64: ori	r0, r25, 0x0018
800fdb68: stw	r0, 0 (r24)
800fdb6c: b	->0x800FDB9C
800fdb70: ori	r0, r25, 0x001A
800fdb74: stw	r0, 0 (r24)
800fdb78: b	->0x800FDB9C
800fdb7c: ori	r0, r25, 0x001B
800fdb80: stw	r0, 0 (r24)
800fdb84: b	->0x800FDB9C
800fdb88: ori	r0, r25, 0x0019
800fdb8c: stw	r0, 0 (r24)
800fdb90: b	->0x800FDB9C
800fdb94: ori	r0, r25, 0x0017
800fdb98: stw	r0, 0 (r24)
800fdb9c: cmpwi	r27, 3
800fdba0: beq-	 ->0x800FDCA8
800fdba4: lbz	r0, 0x02EC (r21) ; Defend Plus
800fdba8: cmplwi	r0, 0
800fdbac: beq-	 ->0x800FDBB4
800fdbb0: add	r28, r28, r0
800fdbb4: lbz	r3, 0x02ED (r21) ; Damage Dodge
800fdbb8: cmplwi	r3, 0
800fdbbc: beq-	 ->0x800FDBCC
800fdbc0: rlwinm.	r0, r25, 0, 13, 13 (00040000)
800fdbc4: beq-	 ->0x800FDBCC
800fdbc8: add	r28, r28, r3
800fdbcc: mr	r3, r21
800fdbd0: lis	r4, 0x0100
800fdbd4: bl	->0x801277F8
800fdbd8: cmpwi	r3, 0
800fdbdc: beq-	 ->0x800FDBE4
800fdbe0: addi	r28, r28, 1 ; "Defend" commands
800fdbe4: mr	r3, r21
800fdbe8: li	r4, 14
800fdbec: bl	->0x80127814
800fdbf0: cmpwi	r3, 0
800fdbf4: beq-	 ->0x800FDC18
800fdbf8: mr	r3, r21
800fdbfc: addi	r5, sp, 9
800fdc00: addi	r6, sp, 8
800fdc04: li	r4, 14
800fdc08: bl	->0x80127F34
800fdc0c: lbz	r0, 0x0008 (sp)
800fdc10: extsb	r0, r0
800fdc14: add	r28, r28, r0
800fdc18: mr	r3, r21
800fdc1c: li	r4, 15
800fdc20: bl	->0x80127814
800fdc24: cmpwi	r3, 0
800fdc28: beq-	 ->0x800FDC54
800fdc2c: mr	r3, r21
800fdc30: addi	r5, sp, 9
800fdc34: addi	r6, sp, 8
800fdc38: li	r4, 15
800fdc3c: bl	->0x80127F34
800fdc40: lbz	r0, 0x0008 (sp)
800fdc44: extsb	r0, r0
800fdc48: add.	r28, r28, r0
800fdc4c: bge-	 ->0x800FDC54
800fdc50: li	r28, 0
800fdc54: addis	r3, r30, 2
800fdc58: lbz	r0, -0x7007 (r3)
800fdc5c: cmplwi	r0, 2
800fdc60: bne-	 ->0x800FDC74
800fdc64: lwz	r0, 0x0008 (r21)
800fdc68: cmpwi	r0, 222
800fdc6c: bne-	 ->0x800FDC74
800fdc70: addi	r28, r28, 3
800fdc74: lwz	r0, 0x01AC (r22)
800fdc78: rlwinm.	r0, r0, 0, 24, 24 (00000080)
800fdc7c: beq-	 ->0x800FDC90
800fdc80: lwz	r0, 0x021C (r21)
800fdc84: add.	r28, r28, r0
800fdc88: bge-	 ->0x800FDC90
800fdc8c: li	r28, 0
800fdc90: lwz	r0, 0x0074 (r23)
800fdc94: rlwinm.	r0, r0, 0, 25, 25 (00000040)
800fdc98: beq-	 ->0x800FDCA8
800fdc9c: cmpwi	r28, 0
800fdca0: ble-	 ->0x800FDCA8
800fdca4: li	r28, 0
800fdca8: cmpwi	r27, 3
800fdcac: sub	r3, r29, r28 ; Attack - Defense
800fdcb0: bne-	 ->0x800FDCB8
800fdcb4: mr	r3, r29
800fdcb8: lbz	r0, 0x02E5 (r21) ; P-Up, D-Down
800fdcbc: add.	r3, r3, r0
800fdcc0: ble-	 ->0x800FDCF8
800fdcc4: lwz	r4, 0x0074 (r23)
800fdcc8: rlwinm.	r0, r4, 0, 27, 27 (00000010)
800fdccc: beq-	 ->0x800FDCE0
800fdcd0: lha	r0, 0x0260 (r21)
800fdcd4: sub.	r3, r3, r0
800fdcd8: bgt-	 ->0x800FDCE0
800fdcdc: li	r3, 1
800fdce0: rlwinm.	r0, r4, 0, 26, 26 (00000020)
800fdce4: beq-	 ->0x800FDCF8
800fdce8: lwz	r0, 0x0280 (r20)
800fdcec: sub.	r3, r3, r0
800fdcf0: bgt-	 ->0x800FDCF8
800fdcf4: li	r3, 1
800fdcf8: rlwinm.	r0, r25, 0, 13, 13 (00040000)
800fdcfc: beq-	 ->0x800FDD04
800fdd00: subi	r3, r3, 1
800fdd04: rlwinm	r0, r26, 0, 24, 31 (000000ff)
800fdd08: cmplwi	r0, 1
800fdd0c: bne-	 ->0x800FDD20
800fdd10: lbz	r0, 0x0301 (r21)
800fdd14: cmplwi	r0, 0
800fdd18: beq-	 ->0x800FDD20
800fdd1c: sub	r3, r3, r0
800fdd20: lbz	r4, 0x02EA (r21) ; Double Pain
800fdd24: lbz	r0, 0x02E9 (r21) ; P-Down, D-Up
800fdd28: cmplwi	r4, 0
800fdd2c: sub	r3, r3, r0
800fdd30: beq-	 ->0x800FDD3C
800fdd34: addi	r0, r4, 1 ; Factor in Double Pain (x N+1)
800fdd38: mullw	r3, r3, r0
800fdd3c: lbz	r6, 0x02EB (r21) ; Last Stand
800fdd40: cmplwi	r6, 0
800fdd44: beq-	 ->0x800FDD6C
800fdd48: lwz	r4, 0x0010 (r21)
800fdd4c: lha	r5, 0x010C (r21)
800fdd50: lbz	r0, 0x000C (r4)
800fdd54: cmpw	r5, r0
800fdd58: bgt-	 ->0x800FDD6C
800fdd5c: addi	r4, r6, 1 ; Factor in Last Stand (/ N+1, rounded up)
800fdd60: subi	r0, r4, 1
800fdd64: add	r0, r3, r0
800fdd68: divw	r3, r0, r4
800fdd6c: cmpwi	r27, 3
800fdd70: beq-	 ->0x800FDDB0
800fdd74: bge-	 ->0x800FDD88
800fdd78: cmpwi	r27, 1
800fdd7c: beq-	 ->0x800FDD94
800fdd80: bge-	 ->0x800FDD9C
800fdd84: b	->0x800FDDB0
800fdd88: cmpwi	r27, 5
800fdd8c: bge-	 ->0x800FDDB0
800fdd90: b	->0x800FDDA4
800fdd94: addi	r3, r3, 1
800fdd98: b	->0x800FDDB0
800fdd9c: li	r3, 0
800fdda0: b	->0x800FDDB0
800fdda4: rlwinm.	r0, r25, 0, 0, 0 (80000000)
800fdda8: bne-	 ->0x800FDDB0
800fddac: li	r3, 0
800fddb0: cmpwi	r3, 0
800fddb4: bge-	 ->0x800FDDBC
800fddb8: li	r3, 0
800fddbc: cmpwi	r27, 3
800fddc0: bne-	 ->0x800FDDD8
800fddc4: lwz	r0, 0 (r24)
800fddc8: mulli	r3, r3, -1
800fddcc: ori	r0, r0, 0x2000
800fddd0: stw	r0, 0 (r24)
800fddd4: b	->0x800FDE18
800fddd8: cmpwi	r3, 0
800fdddc: bgt-	 ->0x800FDDEC
800fdde0: stw	r31, 0 (r24)
800fdde4: li	r3, 0
800fdde8: b	->0x800FDE18
800fddec: rlwinm.	r0, r25, 0, 23, 23 (00000100)
800fddf0: beq-	 ->0x800FDE18
800fddf4: lwz	r0, 0x0074 (r23)
800fddf8: rlwinm.	r0, r0, 0, 16, 16 (00008000)
800fddfc: bne-	 ->0x800FDE0C
800fde00: lwz	r0, 0x0104 (r21)
800fde04: rlwinm.	r0, r0, 0, 23, 23 (00000100)
800fde08: bne-	 ->0x800FDE18
800fde0c: lwz	r0, 0 (r24)
800fde10: oris	r0, r0, 0x0001
800fde14: stw	r0, 0 (r24)
800fde18: lmw	r20, 0x0010 (sp)
800fde1c: lwz	r0, 0x0044 (sp)
800fde20: mtlr	r0
800fde24: addi	sp, sp, 64
800fde28: blr	