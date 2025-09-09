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
    public class DivisionService : IDivisionService
    {

        private readonly IDivisionRepository _divisionRepository;
        private readonly IMapper _mapper;

        public DivisionService()
        {
        }

        public DivisionService(IDivisionRepository divisionRepository) {
            _divisionRepository = divisionRepository;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Division, DivisionViewModel>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public IEnumerable<DivisionViewModel> GetDivisions()
        {
            var result = _divisionRepository.GetAll();
            return _mapper.Map<IEnumerable<DivisionViewModel>>(result);
        }
    }
}
