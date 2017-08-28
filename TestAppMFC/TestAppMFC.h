
// TestAppMFC.h : main header file for the TestAppMFC application
//
#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols


// CTestAppMFCApp:
// See TestAppMFC.cpp for the implementation of this class
//

class CTestAppMFCApp : public CWinApp
{
public:
	CTestAppMFCApp();


// Overrides
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

// Implementation

public:
	afx_msg void OnAppAbout();
	afx_msg void OnAppCheckUpdates();
	DECLARE_MESSAGE_MAP()
};

extern CTestAppMFCApp theApp;
