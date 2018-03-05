#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <math.h>
#include <Windows.h>

#if 0
typedef struct _vec {
	unsigned int length;
	unsigned int isAlloc;
	double* arr;
}vec;

vec vecMake(unsigned int length, double* arr) {
	vec v;
	v.length = length;
	v.isAlloc = 0;
	v.arr = arr;
	return v;
}

vec vecAlloc(unsigned int length) {
	vec v;
	v.length = length;
	v.isAlloc = 1;
	v.arr = (double*)malloc(sizeof(double) * length);
	return v;
}

void freeVec(vec* v) {
	if (v->isAlloc == 1) {
		free(v->arr);
	}
}

void vadd(vec a, vec b, vec* output) {
	assert(a.length == b.length);
	output->length = a.length;
	for (int i = 0; i < a.length; i++) {
		output->arr[i] = a.arr[i] + b.arr[i];
	}
}

double test1(double t, double y) {
	return 2.0*y + t;
}

double test2(double t, double y) {
	return -2.0*y*y + sqrt(t);
}

double test3(double t, double y0, double y1) {
	return -y0;
}

double* badIdea(double t, double x, double y) {
	double* arr = (double*)malloc(sizeof(double) * 2);
	arr[0] = 2 * x + y; arr[1] = -x + 2 * y;
	return arr;
}

void prototype(double t, vec input, vec* output);

typedef struct _timeHeader {
	double tFrom;
	double tTo;
	unsigned int interval;
} timeHeader;

typedef struct _inputHeader {
	/* Time Range */
	timeHeader time;
	/* Initial Value */
	vec initialValues;
} inputHeader;

typedef struct _outputHeader {
	unsigned int length;
	vec* results;
} outputHeader;

outputHeader outputHeaderAlloc(const inputHeader input) {
	outputHeader header;
	header.length = input.time.interval;
	header.results = (vec*)malloc(sizeof(vec) * header.length);
	for (int i = 0; i < header.length; i++) {
		header.results[i] = vecAlloc(input.initialValues.length);
	}
	return header;
}

timeHeader timeHeaderMake(double tFrom, double tTo, unsigned int interval)
{
	timeHeader th;
	th.tFrom = tFrom;
	th.tTo = tTo;
	th.interval = interval;
	return th;
}
inputHeader inputHeaderMake(timeHeader time, vec initialValues)
{
	inputHeader ih;
	ih.time = time;
	ih.initialValues = initialValues;
	return ih;
}

//void freeInputHeader(inputHeader* header)
//{
//	if (header->isAlloc == 1) {
//		free(header->arr);
//	}
//}
void freeOutputHeader(outputHeader* header)
{
	if (header->results != NULL) {
		free(header->results);
	}
}

void solveODE(const inputHeader input, outputHeader* output, void(*func)(double t, vec input, vec* output));
void solveODE(const inputHeader input, outputHeader* output, void(*func)(double t, vec input, vec* output)) {
	// Initialize
	const unsigned int VECLEN = input.initialValues.length;

	const double dt = (input.time.tTo - input.time.tFrom) / input.time.interval;

	double tOld;

	double oldval[VECLEN];
	double newval[VECLEN];

	vec k1 = vecAlloc(VECLEN);
	vec k2 = vecAlloc(VECLEN);
	vec k3 = vecAlloc(VECLEN);
	vec k4 = vecAlloc(VECLEN);

	vec fInput = vecMake(VECLEN, oldval);
	vec fTemp = vecAlloc(VECLEN);

	// Save initial data to output
	for (int i = 0; i < VECLEN; i++) {
		output->results[0].arr[i] = input.initialValues.arr[i];
	}
	// New value is set to the initial data at first
	for (int i = 0; i < VECLEN; i++) {
		newval[i] = output->results[0].arr[i];
	}

	// Iterate and calculate by Runge-Kutta 4th order method
	for (int i = 1; i < input.time.interval; i++) {
		// Move new values to old values
		for (int j = 0; j < VECLEN; j++) {
			oldval[j] = newval[j];
		}

		tOld = input.time.tFrom + (i - 1) * dt;

		// k1 = f(t, Y)
		func(tOld, fInput, &k1);
		vmulScalar(k1, 0.5 * dt, &fTemp);
		vadd(fInput, fTemp, &fTemp);
		// k2 = f(t + 0.5h, y+0.5h*k1)
		func(tOld + 0.5 * dt, fTemp, &k2);
		vmulScalar(k2, 0.5 * dt, &fTemp);
		vadd(fInput, fTemp, &fTemp);
		// k3 = f(t + 0.5h, y+0.5h*k2)
		func(tOld + 0.5 * dt, fTemp, &k3);
		vmulScalar(k3, 1.0 * dt, &fTemp);
		vadd(fInput, fTemp, &fTemp);
		// k4 = f(t + h, y+h*k3)
		func(tOld + 1.0 * dt, fTemp, &k4);

		// Calculate new values
		for (int j = 0; j < VECLEN; j++) {
			newval[j] = oldval[j] + (dt / 6.0) * (1.0 * k1.arr[j] + 2.0 * k2.arr[j] + 2.0 * k3.arr[j] + 1.0 * k4.arr[j]);
		}

		// Save new values to output
		for (int j = 0; j < VECLEN; j++) {
			output->results[i].arr[j] = newval[j];
		}
	}

	// Clean up, and release all the memories used
	freeVec(&k1);
	freeVec(&k2);
	freeVec(&k3);
	freeVec(&k4);
	freeVec(&fInput);
	freeVec(&fTemp);
}

void test01(double t, vec input, vec* output) {
	output->arr[0] = 2.0 * input.arr[0];
}

int main(int argc, const char * argv[]) {
	timeHeader time = timeHeaderMake(0.0, 3.0, 100000);
	vec initialValues = vecAlloc(1);
	initialValues.arr[0] = 1.0;
	inputHeader input = inputHeaderMake(time, initialValues);
	outputHeader output = outputHeaderAlloc(input);

	solveODE(input, &output, test01);

	fileReport(input, output, "Test01");

	//freeInputHeader(&input);
	freeOutputHeader(&output);

	return 0;
}
#endif

#include <direct.h>
#include <io.h>

#define MAX_ARR		1024
int main()
{
	//int arr[MAX_ARR] = { 1000, 0 };
	//int i;
	//
	//for (i = 1; i < MAX_ARR; i++)
	//{
	//	arr[i] = arr[i - 1] + rand() / RAND_MAX * 10 - 5;
	//}

	//for (i = 0; i < MAX_ARR; i++)
	//{
	//	printf("%d ", arr[i]);
	//}

	int ret;
	char buffer[1024];
	char *ptr;
	FILE *fp;
	HANDLE hf;

	ptr = _fullpath(buffer, "test.txt", 1024);
	printf("(0x%x) path = %s\n", ptr, buffer);

	fopen_s(&fp, buffer, "wb");
	printf("%d\n", _access(buffer, 00));
	fclose(fp);
	hf = CreateFile(
		buffer,
		0,
		FILE_SHARE_READ,
		NULL,
		OPEN_EXISTING,
		FILE_FLAG_BACKUP_SEMANTICS,
		NULL);
	if (hf == INVALID_HANDLE_VALUE)
		printf("error %d \n", GetLastError());
	printf("%d\n", _access(buffer, 00));
	//CloseHandle(hf);

	_unlink(buffer);
	printf("%d\n", _access(buffer, 00));
	return 0;
}

