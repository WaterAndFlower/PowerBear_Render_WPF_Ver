#include "pch.h"
#include "Test.h"

int TestDLL(const unsigned long long a, const unsigned long long b) {
	return a + b;
	//return "��C++��DLL���ӳɹ��ˣ�ִ�мӷ����㣺" + std::to_string(a + b);
}

char TestString() {
	return 'A';
}