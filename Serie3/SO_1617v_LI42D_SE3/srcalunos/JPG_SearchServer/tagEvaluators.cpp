#include "stdafx.h"
#include "Date.h"
#include "Rational.h"
 
#include "tagEvaluators.h"


bool TagEval::process(LPVOID value, PSEARCH_EXP exp) {
	if (!strcmp(exp->oper, OP_EQUAL)) return equal(value, exp->value1);
	if (!strcmp(exp->oper, OP_GREATER_THAN)) return greaterThan(value, exp->value1);
	if (!strcmp(exp->oper, OP_LESS_THAN)) return lessThan(value, exp->value1);
	if (!strcmp(exp->oper, OP_BETWEEN)) return between(value, exp->value1, exp->value2);
	if (!strcmp(exp->oper, OP_CONTAINS))
		return true;
	return false;
}


// TagEvalDateTaken

bool TagDateTaken::equal(LPVOID tagValue, char *value) {
	Date dt((char*)tagValue), d(value);

	return dt.compare(d) == 0;
}


bool TagDateTaken::greaterThan(LPVOID tagValue, char *value) {
	Date dt((char*)tagValue), d(value);

	return dt.compare(d) < 0;
 
};

bool  TagDateTaken::lessThan(LPVOID tagValue, char *value) {
	Date dt((char*)tagValue), d(value);

	return dt.compare(d) > 0;
 
}

bool TagDateTaken::between(LPVOID tagValue, char *value1, char *value2) {
	Date dt((char*)tagValue), di(value1), df(value2);

	return dt.isBetween(di, df);
 
}



// TagAperture
bool TagAperture::equal(LPVOID tagValue, char *value) {
	Rational r((int*)tagValue), rv(value);
	return r.compare(rv) == 0;

}

bool TagAperture::greaterThan(LPVOID tagValue, char *value) {
	Rational r((int*)tagValue), rv(value);
	return r.compare(rv) > 0;


};

bool  TagAperture::lessThan(LPVOID tagValue, char *value) {
	Rational r((int*)tagValue), rv(value);
	return r.compare(rv)< 0;

}

bool TagAperture::between(LPVOID tagValue, char *value1, char *value2) {
	Rational r((int*)tagValue), rv1(value1), rv2(value2);
	return r.isBetween(rv1, rv2);
}