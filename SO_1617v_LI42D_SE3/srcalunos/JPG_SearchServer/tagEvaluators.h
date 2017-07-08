#pragma once

#include "searchprocessor.h"

class TagEval {
public:
	virtual bool equal(LPVOID tagValue, char *value) = 0;
	virtual bool greaterThan(LPVOID tagValue, char * value) = 0;
	virtual bool lessThan(LPVOID tagValue, char * value) = 0;
	virtual bool between(LPVOID tagValue, char * value1, char * value2) = 0;
	bool process(LPVOID value, PSEARCH_EXP exp); 
};

class TagDateTaken : public TagEval {
public:
	virtual bool equal(LPVOID tagValue, char * value);
	virtual bool greaterThan(LPVOID tagValue, char * value);
	virtual bool lessThan(LPVOID tagValue, char * value);
	virtual bool between(LPVOID tagValue, char * value1, char * value2);
};

class TagAperture : public TagEval {
public:
	virtual bool equal(LPVOID tagValue, char * value);
	virtual bool greaterThan(LPVOID tagValue, char * value);
	virtual bool lessThan(LPVOID tagValue, char * value);
	virtual bool between(LPVOID tagValue, char * value1, char * value2);
};