using BDA.Identity;
using BDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BDA.Entities;
using BDA.Repository;
using BDA.Interface;

namespace BDA.Service
{
    public class FunctionService : IFunctionService
    {

        private readonly IFunctionRepository _functionRepository;
        private readonly IMapper _mapper;

        public FunctionService()
        {
        }

        public FunctionService(IFunctionRepository functionRepository) {
            _functionRepository = functionRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Function, FunctionViewModel>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public IEnumerable<FunctionViewModel> GetFunctions()
        {
            var result = _functionRepository.GetAll();
            return _mapper.Map<IEnumerable<FunctionViewModel>>(result);
        }
    }
}
