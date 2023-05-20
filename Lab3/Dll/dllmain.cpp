#include "pch.h"
#include <mkl.h>
#include <mkl_df_defines.h>
#include <iostream>


using namespace std;
extern "C"  _declspec(dllexport)


int spline_dll(MKL_INT n, MKL_INT n_1, double* init_NU_grid, double* val_init_NU_grid, double* dif_2, double* st_end,
                double* val_res, double* spline_coef, double* integral, int& code)
{
    try
    {
        DFTaskPtr task;
        int res = dfdNewTask1D(&task, n, init_NU_grid, DF_NON_UNIFORM_PARTITION,
            1, val_init_NU_grid, DF_MATRIX_STORAGE_ROWS);

        // std::cout << "Task created\n";

        res = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL,
            DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER, dif_2, DF_NO_IC, NULL, spline_coef, DF_NO_HINT);

        if (res != DF_STATUS_OK)
        {
            code = res;
            return -1;
        }

        // std::cout << "Spline type defined\n";

        res = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
        
        if (res != DF_STATUS_OK)
        {
            code = res;
            return -1;
        }

        // std::cout << "Spline constructed\n";

        int val_to_count[3]{ 1, 1, 1 };
        res = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, n_1,
            st_end, DF_UNIFORM_PARTITION, 3, val_to_count,
            NULL, val_res, DF_MATRIX_STORAGE_ROWS, NULL);
        
        if (res != DF_STATUS_OK)
        {
            code = res;
            return -1;
        }

        res = dfdIntegrate1D(task, DF_METHOD_PP, 1, new double[1]{st_end[0]}, DF_UNIFORM_PARTITION,
            new double[1]{ st_end[1] }, DF_UNIFORM_PARTITION, NULL, NULL, integral, DF_MATRIX_STORAGE_ROWS);

        // std::cout << "Interpolation finished\n";

        if (res != DF_STATUS_OK)
        {
            code = res;
            return -1;
        }

        return 0;
    }
    catch (...)
    {
        return -1;
    }
}