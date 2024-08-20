package com.example.programa.repository;


import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.transaction.Transactional;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.ArrayList;
import com.example.programa.dto.ProgramaCodDTO;


@Repository
public class ProgramaRepository {

    @PersistenceContext
    private EntityManager em;

    @Transactional
    @SuppressWarnings("unchecked")
    public List<ProgramaCodDTO> getProgramaByCod(String prgcod) {

        String sql = """
                   select
        prgcod as code,
        prgsissiglaalt as sigla,
        prgdsc as description
    from
        programa as prog
    where
        prgcod = :prgcod
    order by
        prog.prgcod
                   """;

        var query = em.createNativeQuery(sql);
        query.setParameter("prgcod", prgcod);

        List<Object[]> rows = query.getResultList();
        List<ProgramaCodDTO> result = new ArrayList<>(rows.size());
        for (Object[] row : rows) {
            result.add(new ProgramaCodDTO((String) row[0], (String) row[1], (String) row[2]));
        }
        return result;
    }

}

