@echo off
@echo This cmd file creates a Data API Builder configuration based on the chosen database objects.
@echo To run the cmd, create an .env file with the following contents:
@echo dab-connection-string=your connection string
@echo ** Make sure to exclude the .env file from source control **
@echo **
dotnet tool install -g Microsoft.DataApiBuilder
dab init -c dab-config.json --database-type mssql --connection-string "@env('dab-connection-string')" --host-mode Development
@echo Adding tables
dab add "TblAssesmentQuestDetail" --source "[dbo].[tblAssesmentQuestDetails]" --fields.include "intid,fk_userid,fk_intEmpAssId,fk_qid,vchAnswer,vchSpecialRemarks,intTotal,vchAssesmentBy,dtAssesment,vchAssesmentHost,vchAssesmentIpused,vchAssUpdatedBy,dtAssUpdated,vchAssUpdatedHost,vchAssUpdatedIpused,BitIsSelected,BitIsCompleated,intcode,intyr" --permissions "anonymous:*" 
dab add "TblAssQuestMa" --source "[dbo].[tblAssQuestMas]" --fields.include "intqid,vchQuestion,vchAnsType,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected" --permissions "anonymous:*" 
dab add "TblCertificateCodeMa" --source "[dbo].[tblCertificateCodeMas]" --fields.include "intid,CertificateCode,intstartFrom,intCurrent,intcode,fk_TemplateId,intYear" --permissions "anonymous:*" 
dab add "TblCity" --source "[dbo].[tblCity]" --fields.include "intid,fk_stateid,vchCityName,vchCreatedBy,dtCreated" --permissions "anonymous:*" 
dab add "TblConsultantDocDetail" --source "[dbo].[tblConsultantDocDetail]" --fields.include "intid,fk_DoctorID,fk_DocMasId,vchDocName,vchDocAddress,vchFileName,dtCreated,vchCreatedBy,vchCretaedIP,vchCreatedHost,dtUpdated,vchUp_By,vchUp_IP,vchUp_Host,bitIsUpload,bitIsLocked,bitIsProfile" --permissions "anonymous:*" 
dab add "TblConsultantLedgerDetail" --source "[dbo].[tblConsultantLedgerDetail]" --fields.include "UUID,fk_EmpID,intYear,fk_LedgerMas,vchFileName,vchFileAddress,vchDocMasName,dtUpload,vchUpBy,intCode,dtUpdated,vchUpDatedBy,vchCompareFile,vchTempMasName" --permissions "anonymous:*" 
dab add "TblConsultantSlipDetail" --source "[dbo].[tblConsultantSlipDetail]" --fields.include "intid,vchMonth,fk_SlipId,intYear,decAmount,decTDS,decNetAmount,vchCreatedBy,dtCreated" --permissions "anonymous:*" 
dab add "TblConsultantSlip" --source "[dbo].[tblConsultantSlips]" --fields.include "UNid,fk_DoctorId,fk_Quarter,fk_MonthNames,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,bitIsRequest,bitIsComplete,bitIsPending,bitIsCancel,intYear,dtRequest,vchRequestedBy,dtComplete,vchCompletedBy,intcode,bitTempComplete,vchRefNumber,vchBranchName,bitIsQuraterRequest,bitIsMonthRequest,intFromMonth,intToMonth" --permissions "anonymous:*" 
dab add "TblConsultantTd" --source "[dbo].[tblConsultantTDS]" --fields.include "UID,fk_EMPID,intYear,fk_QuraterMas,vchFileName,vchFileAddress,vchDocMasName,dtUpload,vchUpBy,intCode,dtUpdated,vchUpDatedBy,vchCompareFile,vchTempMasName" --permissions "anonymous:*" 
dab add "TblConversation" --source "[dbo].[tblConversation]" --fields.include "intid,fk_intEmpid,fk_uid,fk_UserName,vchMsg,dtMsgDate,vchIpused,vchHost,bitIsAuthorCancelReason,intcode,bitIsAutorizationMsg" --permissions "anonymous:*" 
dab add "TblCurrentExpDetail" --source "[dbo].[tblCurrentExpDetail]" --fields.include "intid,fk_MasId,fk_deptId,fk_desiId,vchstate,vchCity,vchAddress,vchRefNo,vchBranch,intCode,vchCreatedBy,DtCeated,vchCreatedHost,vchCratedIp,vchUpdatedBy,dtUpdated,vchUpdatedHost,vchUpdatedIP,vchGender,vchEmpFcode,vchName,vchFatherName,dtdoj,fk_CityId,fkStateId,bitIsDetailAvilable,bitIsDetailUpdated,bitEmpDetailUpdated" --permissions "anonymous:*" 
dab add "TblDeptMa" --source "[dbo].[tblDeptMas]" --fields.include "intid,vchdeptname,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected,vchdepCode,bitIsByPassDept,intManPower,bitIsCounterApplied,bitIsActive,bitIsInActive" --permissions "anonymous:*" 
dab add "TblDesignationMa" --source "[dbo].[tblDesignationMas]" --fields.include "intid,intdeptid,vchdesignation,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected" --permissions "anonymous:*" 
dab add "TblDesiQualification" --source "[dbo].[tblDesiQualification]" --fields.include "intid,fk_desiid,vchQualification,dtcreated,vchCreatedBy,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected" --permissions "anonymous:*" 
dab add "TblDesiSetting" --source "[dbo].[tblDesiSetting]" --fields.include "intid,fk_intdesiid,numExpMin,numExpMax,numSalMin,numSalMax,dtcreated,vchCreatedBy,vchipused,vchhostname,vchupdatedby,vchupdatedipused,vchupdatedhostname,dtupdated,intcode,intyr,IsSelected" --permissions "anonymous:*" 
dab add "TblDnbFeeStructure" --source "[dbo].[TblDnbFeeStructure]" --fields.include "FeeStructureId,DnbStudentId,YearNumber,PayableTo,Amount,DueDate,PaymentStatus,ApprovedBy,ApprovedDate,Remarks" --permissions "anonymous:*" 
dab add "TblDnbFeeSubmission" --source "[dbo].[TblDnbFeeSubmission]" --fields.include "SubmissionId,FeeStructureId,PaymentReferenceNo,PaymentMode,PaymentDate,PaymentScreenshotPath,SubmittedBy,SubmittedDate,VerifiedBy,VerificationDate,VerifiedRemarks,Status,SubmissionRemarks,FileName" --permissions "anonymous:*" 
dab add "TblDnbStudent" --source "[dbo].[TblDnbStudent]" --fields.include "DnbStudentId,fk_EmployeeId,CourseName,DurationInYears,StartDate,EndDate,Status,dtCreated,vchCreatedBy,intcode,intyear" --permissions "anonymous:*" 
dab add "TblDocDetail" --source "[dbo].[tblDocDetails]" --fields.include "intid,fk_empAssid,fk_intdocid,fk_qualiid,vchdocname,vchdocadd,vchfilename,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,intyr,vchipused,vchhostname,fk_NewAssEmpID,BitIsCompDoc,BitIsCompQuali,BitIsProfilePic" --permissions "anonymous:*" 
dab add "TblDocMa" --source "[dbo].[tblDocMas]" --fields.include "intid,vchdocname,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,intyr,vchipused,vchhostname,BitIsSelected" --permissions "anonymous:*" 
dab add "TblDoctorDocMa" --source "[dbo].[tblDoctorDocMas]" --fields.include "intid,vchDocMasName,bitIsComplulsory,bitOtherOption,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchUpdatedHost,bitIsSelected,vchCompareFileAdd,dtTempUploaded,TempEmpID,vchTempFileName" --permissions "anonymous:*" 
dab add "TblDoctorUploadDetail" --source "[dbo].[tblDoctorUploadDetail]" --fields.include "intid,fk_ConsultMasId,bitMOUUploaded,bitRegiUploaded,bitICompletedUpload,Option1,Option2,Option3,bitMOUlocked,bitRegistrationlocked,bitOtherUploadLock,bitisVisiting,bitIsRegular,bitIsActive,bitIsLeft" --permissions "anonymous:*" 
dab add "TblDocTypeMa" --source "[dbo].[tblDocTypeMas]" --fields.include "intTypeId,vchDocTypeName,dtCreated,vchCreatedBy,vchIpUsed,vchHostName,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,BitActive" --permissions "anonymous:*" 
dab add "TblEmpAssesmentDetail" --source "[dbo].[tblEmpAssesmentDetails]" --fields.include "intid,fk_positionid,fk_AssEmpId,fk_userid,vchAssignedToUser,vchCurrentStatus,BitIsAssignCompleted,BitIsSelected,vchCompletedBy,dtCompleted,vchipused,vchhostname,intcode,intyr" --permissions "anonymous:*" 
dab add "TblEmpAssesmentMa" --source "[dbo].[tblEmpAssesmentMas]" --fields.include "intid,vchName,vchMobile,fk_PositionId,decExperience,vchWorkedArea,dtcreated,vchcreatedby,vchcreatedhost,vchcreatedipused,vchAssignedBy,dtAssign,vchAssignhostname,vchAssignipused,vchupdatedby,dtupdated,vchupdatedhostname,vchupdatedipused,BitIsselected,BitCompleteAssesment,BitAssesmentResultPass,vch_Status,BitStatus,BitAllowUpload,BitIsUploadCompleted,decSkillMarks,vchSkillStatus,BitAssesmentResultFail,After,fk_inttitid,vchempcode,vchfname,vchmname,vchlname,fk_intdeptid,fk_intdesiid,vchEmpTcode,vchEmpFcode,dtDOJ,intsalary,dtDOL,bittempstatusactive,bittofficialdetails,bitofficialdetails,bittempqualidetails,bittempperdetails,bittempcontdetails,bitstatusdeactive,bitqualidetails,bitperdetails,bitcontdetails,bitgoauthor,bitauthorised,dtupdatedby,intcode,intyr,vchhostname,ipdaddressused,vchauthorisedby,dtauthorised,vchipdusedauthor,vchautorisedhost,vchdeactiveby,dtdecavited,vchdeactivedipused,vchdeactivedhost,bitauthorcancel,vchcancelreason,vchauthorisedmsg,vchFinalUpdatedBy,dtFinalUpdated,vchFinalHostname,vchFinalipused,bitCompDocT,bitCompDocP,bitProfileComplete,bitIsReplacement,vchRplcmntName,vchAssDeactiveRemarks,vchAssDeactiveBy,vchAssDeactiveDt,bitAuthorBack,vchHrSolRemark,bitIsUnjoined,dtFDOJ,vchUnjoinBy,dtUnjoined,vchArea,bitIsCorporateemp,bitIsUnitEmp,vchEmpOldCode,isHrmsEmployee,isPayrollEmployee,vchgender,bitIsByPassEntry,bitIsPartialAuthorised,BitIsFlaggingEmp,BitIsRedFlagging,BitIsOrangeFlagging,BitIsGreenFlagging,vchAadharNo,vchFlagRemarks,bitOfferLetter,bitExpLetter,bitJoinLetter,bitOfferLGiven,bitExpLGiven,bitJoinLGiven,bitInterShipLetter,bitInterShipGiven,vchUpdatePartialToActive,dtPartialUpdate,bitIsTransferred,bitIsRecieved,bitIsFinalPoliceVeriDone,bitIsTerminated,bitIsLeft,vchTransferRemarks,vchReceiveRemarks,bitIsConsultant,PAN_NO,bitIsCurrentExp,intVariable,intFixed,intTotal,bitIsKRAEligible,dtFirstAuthor,dtApprovedAuthor,dtCreateGetDate,bitAllowExit,bitAssignExitForm,bitExitFormApproved,bitNotCountInManPower,bitIsFwardVp,bitIsAprvdVp,vchPANNUmber" --permissions "anonymous:*" 
dab add "TblEmpCodeMa" --source "[dbo].[tblEmpCodeMas]" --fields.include "intid,vchUnitCode,intStartCode,intCurrentCode,intBranchCode,intJoinYear" --permissions "anonymous:*" 
dab add "TblEmpDetail" --source "[dbo].[tblEmpDetails]" --fields.include "intid,fk_intempid,vchsex,vchmaritalststus,dtDob,intage,vchFatherName,vchmothername,vchspouse,vchtaddress,vchtstate,vchtcity,inttpin,vchtmobile,vchpaddress,vchpstate,vchpcity,vchpstreet,intppin,vchpmobile,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,intyr,vchipused,vchhostname,BitIsSelected,BitCompleted,vchNominee,vchRelation,fk_titid,vchEmpTcode,vchEmpFcode,vchfname,vchmname,vchlname,vchupdatedip,vchupdatedhost,fk_State,fk_city,vchQualifications,fk_tCity,fktState" --permissions "anonymous:*" 
dab add "TblEmpExitDetail" --source "[dbo].[tblEMpExitDetail]" --fields.include "intid,fk_ExitMasid,fk_Qid,vchAnswer,dtCretated,vchCreatedBy,vchCreatedHost,vchCreatedIP,dtUpdated,vchUpdatedBy,vchUpdatedHost,vchUpdatedIP" --permissions "anonymous:*" 
dab add "TblEmpExitMa" --source "[dbo].[tblEmpExitMas]" --fields.include "intid,fkEmpid,vchExitType,vchReasonExit,vchReason2Exit,dtCreated,vchCreatedBY,vchCretaedHost,vchCreatedIp,dtApproved,vchApprovedBy,vchApporvedHost,vchApprovedIP,bitIsCompleted,bitIsApproved" --permissions "anonymous:*" 
dab add "TblEmpLoginUser" --source "[dbo].[tblEmpLoginUser]" --fields.include "intid,fk_intEmpID,vchmobile,vchOTP,dtlastlogin,BitUploadStatus,dtcreated,vchcreatedby,intcode,intyr,bitProfileComplete,bitSigUploaded,vchProfileName,vchProfilePath,vchSigName,vchSigPath,dtUpdated,vchUpdatedBy,bitISAllowedFirstDayLeave" --permissions "anonymous:*" 
dab add "TblEmpMa" --source "[dbo].[tblEmpMas]" --fields.include "intempid,inttitid,vchname,vchempcode,vchEmpTcode,vchEmpFcode,vchfname,vchmname,vchlname,intdeptid,vchdeptname,intdesiid,vchdesignation,dtDOJ,intsalary,dtDOL,bittempstatusactive,bittofficialdetails,bitofficialdetails,bittempqualidetails,bittempperdetails,bittempcontdetails,bitstatusdeactive,bitqualidetails,bitperdetails,bitcontdetails,bitgoauthor,bitauthorised,vchcreatedby,dtcreated,vchupdatedby,dtupdatedby,intcode,intyr,vchhostname,ipdaddressused,BitIsSelected,vchauthorisedby,dtauthorised,vchipdusedauthor,vchautorisedhost,vchdeactiveby,dtdecavited,vchdeactivedipused,vchdeactivedhost,bitauthorcancel,vchcancelreason,vchauthorisedmsg,vchFinalUpdatedBy,dtFinalUpdated,vchFinalHostname,vchFinalipused" --permissions "anonymous:*" 
dab add "TblExitInterviewQuestMa" --source "[dbo].[tblExitInterviewQuestMas]" --fields.include "intid,vchQuestion,dtCreated,vchCreatedBy,vchCretaedHost,vchCreatedIp" --permissions "anonymous:*" 
dab add "TblExperienceDetail" --source "[dbo].[tblExperienceDetail]" --fields.include "intid,fk_Masid,vchName,vchFatherName,fk_designationId,txtContent,dtSdate,dtEdate,dtRelieving,vchCompany,intCode,vchRefCode,vchCreatedBy,dtCreated,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intHRMSid,vchEmpCode,vchState,vchCity,vchAddress,dtServerCreation,bitIshMRMSemp,bitIsEditLock,vchGender,fk_department" --permissions "anonymous:*" 
dab add "TblExperienceMa" --source "[dbo].[tblExperienceMas]" --fields.include "intid,vchName,vchType,vchForGender,vchHeading,txtContent,vchLetterCode,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear" --permissions "anonymous:*" 
dab add "TblGroupMaster" --source "[dbo].[tblGroupMaster]" --fields.include "intid,vchGpName,dtCreated,vchCreatedBy,vchipused,vchHostname,dtUpdated,vchUpdatedBy,vchUpdatedipused,vchupdatedHostname,intcode,intyr" --permissions "anonymous:*" 
dab add "TblIntershipDetail" --source "[dbo].[tblIntershipDetail]" --fields.include "intid,fk_Masid,vchName,vchFatherName,dtApplication,dtDOS,dtDOE,txtContent,vchCompany,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear,dtServerCreation,bitIsHRMSemp,bitIsEditLock,fk_hrmsEMPid,vchState,vchCity,vchAddress,vchEmpCode,fk_department,fk_designation,vchRef_No,vchGender" --permissions "anonymous:*" 
dab add "TblIntershipMa" --source "[dbo].[tblIntershipMas]" --fields.include "intid,vchMasName,txtMasContent,vchLetterCode,vchLetterHeading,vchForGender,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear,dtServerCreation,vchType" --permissions "anonymous:*" 
dab add "TblJoiningRemark" --source "[dbo].[tblJoiningRemarks]" --fields.include "intid,fk_EmpId,vchremarks,vchRemarksBy,dtRemarks,vchHostName,vchIpUsed,bitIsEDOJ,bitIsFDOJ,fk_useridUsed" --permissions "anonymous:*" 
dab add "TblKradocDetail" --source "[dbo].[tblKRADocDetail]" --fields.include "intID,fk_EmpID,vchFileName,vchFileAddress,dtUpload,vchUpBy,itncode,dtUpdated,vchUpDatedBy,vchDocMasName,decFinalScore,decMaxScore,decCohortScore,fk_intDeptid,intTempCohort" --permissions "anonymous:*" 
dab add "TblLeaveApplication" --source "[dbo].[tblLeaveApplication]" --fields.include "unID,fk_Empid,fk_LeaveType,dtStartFrom,dtEndDate,vchReason,vchStatus,vchCreatedBy,dtCreated,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchUpdatedHost,vchApprovedBy,dtApproved,leaveDateRange,decdaysRequest,decdaysApproved,appovedDateRange,bitSameApprove,bitLessApprove,intCode,bitIsApproved,bitIsRejected,bitIsPartialApproved,TempBitAssignedHOD,TempBitHODComplete,TempBitAssignedHR,TempBitHRComplete,TempbitHODNotFound,bitISHalfDay,bitISVpAssigned,bitISVpApproved,vchApprovalType,fk_YearID" --permissions "anonymous:*" 
dab add "TblLeaveApplicationDetail" --source "[dbo].[tblLeaveApplicationDetail]" --fields.include "UN_ID,fk_AppID,vchAssignedUser,fk_AssignUserid,dtAssigned,vchCurrentStatus,bitIsSameApproved,BitIsPartialApproved,BitIsRejected,dtApproved,vchApprovedBy,vchApprovedHost,vchApprovedIP,intCode,vchGetApprovalType,vchApproveRemarks,bitAssignedHOD,bitHODComplete,bitAssignedHR,bitHRComplete,bitHODNotFound,dtApprovedRange,decApprvedDays,bitISApproved,bitISAssigned,bitHalfDay,bitISVpAssigned,bitISVpApproved" --permissions "anonymous:*" 
dab add "TblLeaveBalance" --source "[dbo].[tblLeaveBalance]" --fields.include "unID,fk_Empid,fk_LeaveType,year,intLeaveBalance,vchCreatedBy,dtCreated,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchUpdatedHost,fk_YearID" --permissions "anonymous:*" 
dab add "TblLeaveType" --source "[dbo].[tblLeaveType]" --fields.include "unID,leaveType,vchCreatedBy,dtCreated,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchUpdatedHost,decMaxDayPerYear,bitIsUseInEmgLeave,decMinOnce,decMaxOnce,bitForEmployee,bitForConsultant,bitAllowYearHalfCheck" --permissions "anonymous:*" 
dab add "TblLedgerSession" --source "[dbo].[tblLedgerSession]" --fields.include "intID,vchSession,dtCreated,vchCreatedBy,TempBitIsUploaded,vchTempDocAddress,vchTempFileName,vchTempDocMasName,dtTempUploaded,intTempEmpid" --permissions "anonymous:*" 
dab add "TblLetterAppointDetail" --source "[dbo].[tblLetterAppointDetail]" --fields.include "intid,fk_AppointMasid,vchName,vchFatherName,dtApplication,dtDOJ,txtContent,vchCompany,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear,dtServerCreation,bitIsHRMSemp,bitIsEditLock,fk_hrmsEMPid,vchState,vchCity,vchAddress,vchEmpCode,fk_department,fk_designation,vchRef_No,vchGender,intCTC" --permissions "anonymous:*" 
dab add "TblLetterAppointMa" --source "[dbo].[tblLetterAppointMas]" --fields.include "intid,vchMasType,vchMasName,TextMasContent,vchMasLetterCode,vchMasHeading,vchForGender,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear,dtServerCreation" --permissions "anonymous:*" 
dab add "TblLetterNumberMa" --source "[dbo].[tblLetterNumberMas]" --fields.include "intid,intStart,intCurrent,intcode,intYear" --permissions "anonymous:*" 
dab add "TblLetterOfferDetail" --source "[dbo].[tblLetterOfferDetail]" --fields.include "intid,fk_OfferMas_id,fk_department,fk_designation,vchName,vchFatherName,txtContent,dtAppdate,dtAcceptdate,dtDOJ,vchCompany,intCode,intyr,vchRefCode,fk_HRMS_id,vchEmpCode,vchState,vchCity,vchAddress,vchCreatedBy,dtCreated,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,dtServerCreation,bitIshMRMSemp,bitIsEditLock,vchGender" --permissions "anonymous:*" 
dab add "TblLetterOfferMa" --source "[dbo].[tblLetterOfferMas]" --fields.include "intid,vchName,vchMasType,vchForGender,vchMasHeading,txtMasContent,vchLetterCode,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIp,vchUpdatedHost,intcode,intyear,dtServerCreation" --permissions "anonymous:*" 
dab add "TblManPowerDetail" --source "[dbo].[tblManPowerDetail]" --fields.include "intid,fk_deptid,fk_masID,intManPowerCount,bitAllowManpowerCheck,intCode,dtCreated,vchCreatedBy,vchCreatedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchUpdatedost" --permissions "anonymous:*" 
dab add "TblManPowerMa" --source "[dbo].[tblManPowerMas]" --fields.include "intid,fk_Branch,bitIsmapping,vchCreatedBy,dtCreated,vchCreatedHost,vchCreatedIP,vchUpdatedBy,dtUpdated,vchUpdatedHost,vchUpdatedIP,vchBranchName" --permissions "anonymous:*" 
dab add "TblMonthMaster" --source "[dbo].[tblMonthMaster]" --fields.include "intid,Month,dtCreated,vchCreatedBy,dtUpdated,vchUpdatedBy,bitIsUploaded,vchTempDocAddress,vchTempFileName,vchTempDocMasName,dtTempUploaded,intTempEmpID,decTempScore,decMaxScore,decCohortScore" --permissions "anonymous:*" 
dab add "TblMouDetail" --source "[dbo].[tblMouDetail]" --fields.include "intid,fk_EMpid,dtMOUCreated,dtEffectFrom,dtEffectTo,bitIsOLD,bitIsCurrent,bitIsRenew,dtCretaed,vchCreatedBy,vchCretaedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,vchFileName,vchFileAddress,vchDocName" --permissions "anonymous:*" 
dab add "TblOtherDocDetail" --source "[dbo].[tblOtherDocDetail]" --fields.include "intid,fk_empAssid,fk_intOtherdocid,vchDocName,vchdocadd,vchfilename,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,vchipused,vchhostname,fk_NewAssEmpID,BitISInductionDoc,vchNameDisplay" --permissions "anonymous:*" 
dab add "TblOtherDocumentMa" --source "[dbo].[tblOtherDocumentMas]" --fields.include "intid,vchDocName,bitISMultipleRecords,dtCretaed,vchCretaedBy,vchCretaedHost,vchCretaedIP,dtUpdated,vchUpdatedBy,vchUpdatedHost,vchUpdatedIP,intcode,bitIsSelected,vchCompareFileAdd,dtTempUploaded,TempEmpID,vchTempDocName" --permissions "anonymous:*" 
dab add "TblPayrollDatum" --source "[dbo].[tblPayrollData]" --fields.include "intid,Name,dtDOJ,vchMobile,vchEMpCode,vchGender,vchFatherName,vchMotherName,vchSpouseName,intCode,BitInHRMS,AadhaarNo" --permissions "anonymous:*" 
dab add "TblPermissionMaster" --source "[dbo].[tblPermissionMaster]" --fields.include "intid,fk_GName,fk_Gpid,vchPermissionName,dtCreated,vchCreatedBy,vchipused,vchHostname,dtUpdated,vchUpdatedBy,vchUpdatedipused,vchupdatedHostname,bitIsselected,intcode,intyr" --permissions "anonymous:*" 
dab add "TblPosDesiMap" --source "[dbo].[tblPosDesiMap]" --fields.include "intid,fk_PosCatid,fk_desiid,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected,bitIsLastAssessment" --permissions "anonymous:*" 
dab add "TblPosDocMap" --source "[dbo].[tblPosDocMap]" --fields.include "intid,fk_PosCatid,fk_docid,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected,BitIsUploaded,ComparedFileName,fk_TempEmpId,fk_TempCompDocid,dt_TempUploaded,bitRequireForAuthorization,bitRequireForPartialToComplete" --permissions "anonymous:*" 
dab add "TblPositionCategoryMa" --source "[dbo].[tblPositionCategoryMas]" --fields.include "intid,vchPosCatName,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipused,vchupdatedhostname,intcode,intyr,IsSelected,BitDesiMapping,BitSkillMapping,BitDocMapping" --permissions "anonymous:*" 
dab add "TblQualiDetail" --source "[dbo].[tblQualiDetails]" --fields.include "intid,intempid,fk_intqualiid,vchqname,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,intyr,vchipused,vchhostname,bitissleceted" --permissions "anonymous:*" 
dab add "TblQualiMa" --source "[dbo].[tblQualiMas]" --fields.include "intqualiid,vchqname,vchcreatedby,dtcreated,vchupdatedby,dtupdated,intcode,intyr,vchipused,vchhostname,BitIsSlected,BitIsFinalDoc" --permissions "anonymous:*" 
dab add "TblQuraterMa" --source "[dbo].[tblQuraterMas]" --fields.include "intid,vchMonth,intQurater,dtCreated,TempBitIsUploaded,vchTempDocAddress,vchTempFileName,vchTempDocMasName,dtTempUploaded,intTempEmpid,vchTempYear" --permissions "anonymous:*" 
dab add "TblRegistrationDetail" --source "[dbo].[tblRegistrationDetail]" --fields.include "intid,fk_EMpid,dtRegistration,dtRegistrationFrom,dtRegistrationTo,bitIsOld,bitIsNew,vchFileName,vchFileAddress,vchDocName,dtCretaed,vchCreatedBy,vchCretaedIP,vchCreatedHost,dtUpdated,vchUpdatedBy,vchUpdatedIP,bitIsRenew" --permissions "anonymous:*" 
dab add "TblRequisition" --source "[dbo].[tblRequisition]" --fields.include "intid,vchReqType,BitIsReplacement,fkReplacement_empid,fk_PositionID,vchEducation,vchAditionalEducation,vchSkill,intExpRequired,dtRequiredDate,dtRequest,vchRequestBy,bitRequestByMgr,bitRequestByHR,bitRequestByVPHR,vchRequestIP,vchRequestHost,btiISAssignFromMgr,bitISAssignFromHR,bitISAssignFromVPHR,bitISAssignToHR,bitISAssignToVPHR,bitISAssignToCFO,bitISForwardFromHR,dtForwardFromHR,bitISForwardFromVPHR,dtForwardFromVPHR,bitISApprovedByHR,dtApprovedByHR,bitISApprvoedByVPHR,dtApprovedByVPHR,bitISApprovedByCFO,dtApprovedByCFO,bitStatusForward,bitStatusPending,bitStatusComplete,vchTypeName,bitISNew,intCode,vchRemarks,vchApprovalRemarks,bitStatusCancel,dtCancel,vchCancelBy,bitCancelByHR,bitCancelByVPHR,bitCancelByCFO,dtApproved,vchApprovedBy,vchRNumber,vchRplcMentName,vchRplcMentEmpCode" --permissions "anonymous:*" 
dab add "TblRequisitionNumberMa" --source "[dbo].[tblRequisitionNumberMas]" --fields.include "intid,intStart,intCurrent,dtstart" --permissions "anonymous:*" 
dab add "TblSalaryIncrement" --source "[dbo].[tblSalaryIncrement]" --fields.include "intid,fk_Empid,intOldSalary,intIncrementAmt,intNetSalary,vchUpby,dtUpdate,vchUser" --permissions "anonymous:*" 
dab add "TblSkillMarksAndPositionMappMa" --source "[dbo].[tblSkillMarksAndPositionMappMas]" --fields.include "intid,fk_PositionID,intSkillMarksFm,intSkillMarksTo,vchCreatedBy,dtCreated,vchIpUsed,vchHostname,dtUpdated,vchUpdatedBy,vchUpdatedIpUsed,vchUpdatedHost,intCode,intYear,BitMapped,BitIsSelected" --permissions "anonymous:*" 
dab add "TblState" --source "[dbo].[tblState]" --fields.include "intid,vchState,vchcreatedBy,dtCreated,bitIsActive,bitIsUT" --permissions "anonymous:*" 
dab add "TblTitleMa" --source "[dbo].[tblTitleMas]" --fields.include "intid,vchname,vchcreatedby,dtcreated,vchupdatedby,dtupdate,vchipused,vchhostname,intcode,intyr,IsSelected" --permissions "anonymous:*" 
dab add "TblTransferDetail" --source "[dbo].[tblTransferDetail]" --fields.include "intid,fk_empid,vchTransferFromBranch,vchTransferToBranch,dtRelieved,dtTransferredDOJ,intTransferSalary,intRecievedSalary,vchTransferBy,dtTransfer,vchTransferHost,vchTransferIP,vchRecievedBy,dtRecieved,vchRecievedHost,vchRecievedIP,vchTransferRemarks,vchRecievedRemarks,BitCancel,vchCancelReason,fk_transferDept,fk_TransferDesi,fk_ReacievedDept,fk_RecievedDesi,intTransferredCode,intRecievedCode,vchOldEmpCode,intTransferToCode,BitTransferComplete" --permissions "anonymous:*" 
dab add "TblUserAuthorize" --source "[dbo].[tblUserAuthorize]" --fields.include "intID,vchUserName,bitAuthorization,intSalaryfrom,intSalaryTo,vchcreatedby,dtcreated,vchipused,vchhostname,vchupdatedby,dtupdated,vchupdatedipdused,vchupdatedhostname,intcode,intyr" --permissions "anonymous:*" 
dab add "TblUserLove" --source "[dbo].[tblUserLove]" --fields.include "intid,fk_intUserId,BitMainAdmin,BitHrAdmin,BitOtherOption2,BitOtherOption3,BitOtherOption4,intcode,intyr,IsVPHR,dtFromSession,bitNABHAccount" --permissions "anonymous:*" 
dab add "TblUserMaster" --source "[dbo].[tblUserMaster]" --fields.include "intid,vchUsername,Passcode,bitActive,vchDepartment,fk_intDeptid,vchDesignation,fk_intDesignationid,vchMobile,bitAllowAssesment,vchcreatedby,dtcreated,vchhostname,vchipused,vchupdatedby,dtupdated,vchupdatedhostname,vchupdatedipused,intcode,intyr,vchEmail,bitIsAllowLastAss,vchProfilePath,vchSignaturePath,vchProfileName,vchSignatureName,bitProfileComplete,bitSigComplete,bitISHOD" --permissions "anonymous:*" 
dab add "TblUserPermission" --source "[dbo].[tblUserPermission]" --fields.include "intid,fk_group,fk_gpid,fk_permissionname,vchUserName,bitAllowed,fk_Permissionid,dtCreated,vchCreatedBy,vchipused,vchHostname,dtUpdated,vchUpdatedBy,vchUpdatedipused,vchupdatedHostname,bitIsSelected,intcode,intyr" --permissions "anonymous:*" 
dab add "TblVpforward" --source "[dbo].[tblVPForward]" --fields.include "intid,fk_EmpID,dtForwarded,vchForwardBy,bitStatus,dtApproved,vchApprovedBy,bitCancel" --permissions "anonymous:*" 
dab add "TblYearMaster" --source "[dbo].[tblYearMaster]" --fields.include "unID,vchYearName,dtStartDate,dtEndDate,bitIsActive" --permissions "anonymous:*" 
@echo Adding views and tables without primary key
@echo No primary key found for table/view 'tblBranchShiftMas', using first Id column (intid) as key field
dab add "TblBranchShiftMaView" --source "[dbo].[tblBranchShiftMas]" --fields.include "intid,fk_empid,fk_oldBranchId,fk_newBranchId,dtShifted,vchShiftedBy,vchIpdUsed,vchHost,fk_UsedUserid" --source.type "view" --source.key-fields "intid" --permissions "anonymous:*" 
@echo No primary key found for table/view 'tblDeletedData', using first Id column (TID) as key field
dab add "TblDeletedDatumView" --source "[dbo].[tblDeletedData]" --fields.include "CLSN,Op,TID,begTime,TName,TSID" --source.type "view" --source.key-fields "TID" --permissions "anonymous:*" 
@echo No primary key found for table/view 'tblDeleteRows', using first Id column (Transaction ID) as key field
dab add "TblDeleteRowView" --source "[dbo].[tblDeleteRows]" --fields.include "Current LSN,Transaction ID,Operation,Context,AllocUnitName" --source.type "view" --source.key-fields "Transaction ID" --permissions "anonymous:*" 
@echo Adding relationships
dab update TblAssesmentQuestDetail --relationship TblAssQuestMa --target.entity TblAssQuestMa --cardinality one
dab update TblAssQuestMa --relationship TblAssesmentQuestDetail --target.entity TblAssesmentQuestDetail --cardinality many
dab update TblAssesmentQuestDetail --relationship TblUserMaster --target.entity TblUserMaster --cardinality one
dab update TblUserMaster --relationship TblAssesmentQuestDetail --target.entity TblAssesmentQuestDetail --cardinality many
dab update TblBranchShiftMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblBranchShiftMa --target.entity TblBranchShiftMa --cardinality many
dab update TblCity --relationship TblState --target.entity TblState --cardinality one
dab update TblState --relationship TblCity --target.entity TblCity --cardinality many
dab update TblConsultantDocDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblConsultantDocDetail --target.entity TblConsultantDocDetail --cardinality many
dab update TblConsultantLedgerDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblConsultantLedgerDetail --target.entity TblConsultantLedgerDetail --cardinality many
dab update TblConsultantSlipDetail --relationship TblConsultantSlip --target.entity TblConsultantSlip --cardinality one
dab update TblConsultantSlip --relationship TblConsultantSlipDetail --target.entity TblConsultantSlipDetail --cardinality many
dab update TblConsultantSlip --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblConsultantSlip --target.entity TblConsultantSlip --cardinality many
dab update TblConsultantTd --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblConsultantTd --target.entity TblConsultantTd --cardinality many
dab update TblConsultantTd --relationship TblQuraterMa --target.entity TblQuraterMa --cardinality one
dab update TblQuraterMa --relationship TblConsultantTd --target.entity TblConsultantTd --cardinality many
dab update TblConversation --relationship TblUserMaster --target.entity TblUserMaster --cardinality one
dab update TblUserMaster --relationship TblConversation --target.entity TblConversation --cardinality many
dab update TblCurrentExpDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblCurrentExpDetail --target.entity TblCurrentExpDetail --cardinality many
dab update TblCurrentExpDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblCurrentExpDetail --target.entity TblCurrentExpDetail --cardinality many
dab update TblCurrentExpDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblCurrentExpDetail --target.entity TblCurrentExpDetail --cardinality many
dab update TblDesignationMa --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality many
dab update TblDesiQualification --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblDesiQualification --target.entity TblDesiQualification --cardinality many
dab update TblDesiSetting --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblDesiSetting --target.entity TblDesiSetting --cardinality many
dab update TblDnbFeeStructure --relationship TblDnbStudent --target.entity TblDnbStudent --cardinality one
dab update TblDnbStudent --relationship TblDnbFeeStructure --target.entity TblDnbFeeStructure --cardinality many
dab update TblDnbFeeSubmission --relationship TblDnbFeeStructure --target.entity TblDnbFeeStructure --cardinality one
dab update TblDnbFeeStructure --relationship TblDnbFeeSubmission --target.entity TblDnbFeeSubmission --cardinality many
dab update TblDnbStudent --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblDnbStudent --target.entity TblDnbStudent --cardinality many
dab update TblDocDetail --relationship TblDocMa --target.entity TblDocMa --cardinality one
dab update TblDocMa --relationship TblDocDetail --target.entity TblDocDetail --cardinality many
dab update TblDocDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblDocDetail --target.entity TblDocDetail --cardinality many
dab update TblDoctorUploadDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblDoctorUploadDetail --target.entity TblDoctorUploadDetail --cardinality many
dab update TblEmpAssesmentDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblEmpAssesmentDetail --target.entity TblEmpAssesmentDetail --cardinality many
dab update TblEmpAssesmentDetail --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblEmpAssesmentDetail --target.entity TblEmpAssesmentDetail --cardinality many
dab update TblEmpAssesmentDetail --relationship TblUserMaster --target.entity TblUserMaster --cardinality one
dab update TblUserMaster --relationship TblEmpAssesmentDetail --target.entity TblEmpAssesmentDetail --cardinality many
dab update TblEmpAssesmentMa --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality many
dab update TblEmpAssesmentMa --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality many
dab update TblEmpAssesmentMa --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality many
dab update TblEmpAssesmentMa --relationship TblTitleMa --target.entity TblTitleMa --cardinality one
dab update TblTitleMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality many
dab update TblEmpDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblEmpDetail --target.entity TblEmpDetail --cardinality many
dab update TblEmpDetail --relationship TblTitleMa --target.entity TblTitleMa --cardinality one
dab update TblTitleMa --relationship TblEmpDetail --target.entity TblEmpDetail --cardinality many
dab update TblEmpExitDetail --relationship TblEmpExitMa --target.entity TblEmpExitMa --cardinality one
dab update TblEmpExitMa --relationship TblEmpExitDetail --target.entity TblEmpExitDetail --cardinality many
dab update TblEmpExitDetail --relationship TblExitInterviewQuestMa --target.entity TblExitInterviewQuestMa --cardinality one
dab update TblExitInterviewQuestMa --relationship TblEmpExitDetail --target.entity TblEmpExitDetail --cardinality many
dab update TblEmpExitMa --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblEmpExitMa --target.entity TblEmpExitMa --cardinality many
dab update TblEmpLoginUser --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblEmpLoginUser --target.entity TblEmpLoginUser --cardinality many
dab update TblEmpMa --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblEmpMa --target.entity TblEmpMa --cardinality many
dab update TblEmpMa --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblEmpMa --target.entity TblEmpMa --cardinality many
dab update TblEmpMa --relationship TblTitleMa --target.entity TblTitleMa --cardinality one
dab update TblTitleMa --relationship TblEmpMa --target.entity TblEmpMa --cardinality many
dab update TblExperienceDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblExperienceDetail --target.entity TblExperienceDetail --cardinality many
dab update TblExperienceDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblExperienceDetail --target.entity TblExperienceDetail --cardinality many
dab update TblExperienceDetail --relationship TblExperienceMa --target.entity TblExperienceMa --cardinality one
dab update TblExperienceMa --relationship TblExperienceDetail --target.entity TblExperienceDetail --cardinality many
dab update TblIntershipDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblIntershipDetail --target.entity TblIntershipDetail --cardinality many
dab update TblIntershipDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblIntershipDetail --target.entity TblIntershipDetail --cardinality many
dab update TblIntershipDetail --relationship TblIntershipMa --target.entity TblIntershipMa --cardinality one
dab update TblIntershipMa --relationship TblIntershipDetail --target.entity TblIntershipDetail --cardinality many
dab update TblJoiningRemark --relationship TblUserMaster --target.entity TblUserMaster --cardinality one
dab update TblUserMaster --relationship TblJoiningRemark --target.entity TblJoiningRemark --cardinality many
dab update TblKradocDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblKradocDetail --target.entity TblKradocDetail --cardinality many
dab update TblLeaveApplication --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblLeaveApplication --target.entity TblLeaveApplication --cardinality many
dab update TblLeaveApplication --relationship TblLeaveType --target.entity TblLeaveType --cardinality one
dab update TblLeaveType --relationship TblLeaveApplication --target.entity TblLeaveApplication --cardinality many
dab update TblLeaveApplicationDetail --relationship TblLeaveApplication --target.entity TblLeaveApplication --cardinality one
dab update TblLeaveApplication --relationship TblLeaveApplicationDetail --target.entity TblLeaveApplicationDetail --cardinality many
dab update TblLetterAppointDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblLetterAppointDetail --target.entity TblLetterAppointDetail --cardinality many
dab update TblLetterAppointDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblLetterAppointDetail --target.entity TblLetterAppointDetail --cardinality many
dab update TblLetterAppointDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblLetterAppointDetail --target.entity TblLetterAppointDetail --cardinality many
dab update TblLetterAppointDetail --relationship TblLetterAppointMa --target.entity TblLetterAppointMa --cardinality one
dab update TblLetterAppointMa --relationship TblLetterAppointDetail --target.entity TblLetterAppointDetail --cardinality many
dab update TblLetterOfferDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblLetterOfferDetail --target.entity TblLetterOfferDetail --cardinality many
dab update TblLetterOfferDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblLetterOfferDetail --target.entity TblLetterOfferDetail --cardinality many
dab update TblLetterOfferDetail --relationship TblLetterOfferMa --target.entity TblLetterOfferMa --cardinality one
dab update TblLetterOfferMa --relationship TblLetterOfferDetail --target.entity TblLetterOfferDetail --cardinality many
dab update TblManPowerDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblManPowerDetail --target.entity TblManPowerDetail --cardinality many
dab update TblManPowerDetail --relationship TblManPowerMa --target.entity TblManPowerMa --cardinality one
dab update TblManPowerMa --relationship TblManPowerDetail --target.entity TblManPowerDetail --cardinality many
dab update TblMouDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblMouDetail --target.entity TblMouDetail --cardinality many
dab update TblOtherDocDetail --relationship TblOtherDocumentMa --target.entity TblOtherDocumentMa --cardinality one
dab update TblOtherDocumentMa --relationship TblOtherDocDetail --target.entity TblOtherDocDetail --cardinality many
dab update TblPermissionMaster --relationship TblGroupMaster --target.entity TblGroupMaster --cardinality one
dab update TblGroupMaster --relationship TblPermissionMaster --target.entity TblPermissionMaster --cardinality many
dab update TblPosDesiMap --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblPosDesiMap --target.entity TblPosDesiMap --cardinality many
dab update TblPosDesiMap --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblPosDesiMap --target.entity TblPosDesiMap --cardinality many
dab update TblPosDocMap --relationship TblDocMa --target.entity TblDocMa --cardinality one
dab update TblDocMa --relationship TblPosDocMap --target.entity TblPosDocMap --cardinality many
dab update TblPosDocMap --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblPosDocMap --target.entity TblPosDocMap --cardinality many
dab update TblQualiDetail --relationship TblQualiMa --target.entity TblQualiMa --cardinality one
dab update TblQualiMa --relationship TblQualiDetail --target.entity TblQualiDetail --cardinality many
dab update TblRegistrationDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblRegistrationDetail --target.entity TblRegistrationDetail --cardinality many
dab update TblRequisition --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblRequisition --target.entity TblRequisition --cardinality many
dab update TblSalaryIncrement --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblSalaryIncrement --target.entity TblSalaryIncrement --cardinality many
dab update TblSkillMarksAndPositionMappMa --relationship TblPositionCategoryMa --target.entity TblPositionCategoryMa --cardinality one
dab update TblPositionCategoryMa --relationship TblSkillMarksAndPositionMappMa --target.entity TblSkillMarksAndPositionMappMa --cardinality many
dab update TblTransferDetail --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblTransferDetail --target.entity TblTransferDetail --cardinality many
dab update TblTransferDetail --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblTransferDetail --target.entity TblTransferDetail --cardinality many
dab update TblTransferDetail --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblTransferDetail --target.entity TblTransferDetail --cardinality many
dab update TblUserLove --relationship TblUserMaster --target.entity TblUserMaster --cardinality one
dab update TblUserMaster --relationship TblUserLove --target.entity TblUserLove --cardinality many
dab update TblUserMaster --relationship TblDeptMa --target.entity TblDeptMa --cardinality one
dab update TblDeptMa --relationship TblUserMaster --target.entity TblUserMaster --cardinality many
dab update TblUserMaster --relationship TblDesignationMa --target.entity TblDesignationMa --cardinality one
dab update TblDesignationMa --relationship TblUserMaster --target.entity TblUserMaster --cardinality many
dab update TblUserPermission --relationship TblGroupMaster --target.entity TblGroupMaster --cardinality one
dab update TblGroupMaster --relationship TblUserPermission --target.entity TblUserPermission --cardinality many
dab update TblUserPermission --relationship TblPermissionMaster --target.entity TblPermissionMaster --cardinality one
dab update TblPermissionMaster --relationship TblUserPermission --target.entity TblUserPermission --cardinality many
dab update TblVpforward --relationship TblEmpAssesmentMa --target.entity TblEmpAssesmentMa --cardinality one
dab update TblEmpAssesmentMa --relationship TblVpforward --target.entity TblVpforward --cardinality many
@echo Adding stored procedures
dab add "SpGetDnbFeeDashboard" --source "[dbo].[sp_GetDnbFeeDashboard]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetDnbFeeReport" --source "[dbo].[sp_GetDnbFeeReport]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpCurrentExpCertificate" --source "[dbo].[spCurrentExpCertificate]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEditPosDesiMap" --source "[dbo].[spEditPosDesiMap]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEditPosDocMap" --source "[dbo].[spEditPosDocMap]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEmpAppointLetter" --source "[dbo].[spEmpAppointLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEmpExpLetter" --source "[dbo].[spEmpExpLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEmpinternshipLetter" --source "[dbo].[spEmpinternshipLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpEmpOfferLetter" --source "[dbo].[spEmpOfferLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetActiveConsultant" --source "[dbo].[spGetActiveConsultant]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetAssesmentRemark" --source "[dbo].[spGetAssesmentRemarks]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetBranchMapping" --source "[dbo].[spGetBranchMapping]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetCohort" --source "[dbo].[spGetCohort]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetConsultant" --source "[dbo].[spGetConsultant]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetConsultantSlip" --source "[dbo].[spGetConsultantSlip]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetConsultantSlipDetail" --source "[dbo].[spGetConsultantSlipDetail]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetConsumedConsultantLeave" --source "[dbo].[spGetConsumedConsultantLeave]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetConsumedLeave" --source "[dbo].[spGetConsumedLeave]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetDeptMapping" --source "[dbo].[spGetDeptMapping]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetdoj" --source "[dbo].[spGetdoj]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetdoj123" --source "[dbo].[spGetdoj123]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetdol" --source "[dbo].[spGetdol]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetEmpDatum" --source "[dbo].[spGetEmpData]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetInternshipLetter" --source "[dbo].[spGetInternshipLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetLeaveCounter" --source "[dbo].[spGetLeaveCounter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetOfferLetter" --source "[dbo].[spGetOfferLetter]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetPartialAuthorizedEmp" --source "[dbo].[spGetPartialAuthorizedEmp]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetRequisition" --source "[dbo].[spGetRequisition]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetSearchEmployee" --source "[dbo].[spGetSearchEmployee]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpGetUserPermission" --source "[dbo].[spGetUserPermission]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpLeaveReport" --source "[dbo].[spLeaveReport]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
dab add "SpLeaveReport123" --source "[dbo].[spLeaveReport123]" --source.type "stored-procedure" --permissions "anonymous:execute" --rest.methods "get" --graphql.operation "query" 
@echo **
@echo ** run 'dab validate' to validate your configuration **
@echo ** run 'dab start' to start the development API host **
